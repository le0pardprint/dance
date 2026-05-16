if (!getToken() || !getRole()) {
  window.location.href = 'login.html';
}

const ROLE   = getRole();
const EMAIL  = getEmail();

const ROLE_LABELS = {
  Client: 'Клиент', Trainer: 'Тренер',
  Director: 'Директор', Accountant: 'Бухгалтер', Admin: 'Администратор',
};

const PAGE_TITLES = {
  Client: 'Личный кабинет', Trainer: 'Кабинет тренера',
  Director: 'Аналитика', Accountant: 'Финансы', Admin: 'Управление',
};

const ROLE_NAV = {
  Client:    [{ page: 'client',     icon: '🏠', label: 'Личный кабинет' }],
  Trainer:   [{ page: 'trainer',    icon: '📋', label: 'Кабинет тренера' }],
  Director:  [{ page: 'director',   icon: '📊', label: 'Аналитика' }],
  Accountant:[{ page: 'accountant', icon: '💰', label: 'Финансы' }],
  Admin:     [{ page: 'admin',      icon: '⚙️', label: 'Управление' }],
};

document.getElementById('api-label').textContent  = API_URL;
document.getElementById('page-title').textContent = PAGE_TITLES[ROLE] ?? 'Главная';
document.getElementById('sb-email').textContent   = EMAIL ?? '—';
document.getElementById('sb-role').textContent    = (ROLE_LABELS[ROLE] ?? ROLE);
document.getElementById('sb-avatar').textContent  = EMAIL ? EMAIL[0].toUpperCase() : '?';

const navEl = document.getElementById('sidebar-nav');
(ROLE_NAV[ROLE] ?? []).forEach(item => {
  const el = document.createElement('div');
  el.className = 'nav-item active';
  el.innerHTML = `<span class="nav-icon">${item.icon}</span>${item.label}`;
  navEl.appendChild(el);
});

document.querySelectorAll('.page').forEach(p => p.classList.remove('active'));
const rolePage = document.getElementById(`page-${ROLE.toLowerCase()}`);
if (rolePage) rolePage.classList.add('active');

document.getElementById('logout-btn').addEventListener('click', () => {
  clearAuth();
  window.location.href = 'login.html';
});

document.querySelectorAll('.tabs').forEach(tabsEl => {
  tabsEl.querySelectorAll('.tab-btn').forEach(btn => {
    btn.addEventListener('click', () => {
      const paneId = btn.dataset.tab;
      const card   = btn.closest('.card-body') || btn.closest('.card');
      tabsEl.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
      btn.classList.add('active');
      card.querySelectorAll('.tab-pane').forEach(p => p.classList.remove('active'));
      const pane = document.getElementById(paneId);
      if (pane) {
        pane.classList.add('active');
        if (!pane.dataset.loaded) loadPane(paneId);
      }
    });
  });
});

const cache = {};

async function fetchCached(key, url) {
  if (cache[key]) return cache[key];
  const { ok, status, data } = await apiFetch(url);
  if (ok) cache[key] = data;
  return ok ? data : null;
}

async function loadPane(paneId) {
  const el = document.getElementById(paneId);
  if (!el) return;
  el.dataset.loaded = '1';
  showLoading(el);

  switch (paneId) {
    case 'client-schedule': {
      const data = await fetchCached('clientSchedule', '/api/Client/schedule');
      if (!data) { showErr(el, 'Не удалось загрузить расписание'); return; }
      updateClientStats(data, null, null);
      el.innerHTML = buildTable(
        ['Дата','Время','Группа','Направление','Тренер','Статус'],
        data.map(c => [fmtDate(c.date), c.time??'—', c.groupName??'—', c.directionName??'—', c.trainerName??'—', badge(c.status)]),
        'Расписание пусто'
      );
      break;
    }
    case 'client-groups': {
      const data = await fetchCached('clientGroups', '/api/Client/my-groups');
      if (!data) { showErr(el, 'Не удалось загрузить группы'); return; }
      updateClientStats(null, data, null);
      el.innerHTML = buildTable(
        ['Группа','Направление','Тренер','Статус'],
        data.map(g => [g.name, g.directionName??'—', g.trainerName??'—', badge(g.status)]),
        'Нет групп'
      );
      break;
    }
    case 'client-payments': {
      const data = await fetchCached('clientPayments', '/api/Client/payments');
      if (!data) { showErr(el, 'Не удалось загрузить платежи'); return; }
      updateClientStats(null, null, data);
      const statsHtml = buildStats([
        [fmtMoney(data.totalPaid), 'Оплачено'],
        [fmtMoney(data.totalDebt), 'К оплате'],
      ]);
      const tableHtml = buildTable(
        ['#','Группа','Сумма','Статус'],
        data.subscriptions?.map((s,i) => [i+1, s.groupName??'—', fmtMoney(s.amount), badge(s.status)]),
        'Нет абонементов'
      );
      el.innerHTML = statsHtml + tableHtml;
      break;
    }
    case 'trainer-schedule': {
      const data = await fetchCached('trainerSchedule', '/api/Trainer/schedule');
      if (!data) { showErr(el, 'Не удалось загрузить расписание'); return; }
      updateTrainerStats(data, null);
      el.innerHTML = buildTable(
        ['Дата','Время','Группа','Учеников','Статус'],
        data.map(c => [fmtDate(c.date), c.time??'—', c.groupName??'—', c.studentsCount??'—', badge(c.status)]),
        'Расписание пусто'
      );
      break;
    }
    case 'trainer-groups': {
      const data = await fetchCached('trainerGroups', '/api/Trainer/my-groups');
      if (!data) { showErr(el, 'Не удалось загрузить группы'); return; }
      updateTrainerStats(null, data);
      el.innerHTML = buildTrainerGroupsTable(data);
      el.querySelectorAll('.expand-btn').forEach(btn => {
        btn.addEventListener('click', () => {
          const idx      = btn.dataset.idx;
          const subRows  = el.querySelectorAll(`.sub-rows-${idx}`);
          const expanded = btn.dataset.expanded === '1';
          subRows.forEach(r => r.style.display = expanded ? 'none' : '');
          btn.dataset.expanded = expanded ? '0' : '1';
          btn.textContent = expanded ? '▼ Ученики' : '▲ Скрыть';
        });
      });
      break;
    }
    case 'dir-occupancy': {
      const data = await fetchCached('dirOccupancy', '/api/Analytics/groups/occupancy');
      if (!data) { showErr(el, 'Ошибка загрузки'); return; }
      document.getElementById('director-stats').innerHTML = buildStats([
        [data.totalGroups, 'Групп'],
        [data.totalStudents, 'Учеников'],
        [data.averageOccupancyPercent + '%', 'Ср. заполненность'],
      ]);
      el.innerHTML = buildTable(
        ['Группа','Статус','Учеников','Макс.','Заполненность'],
        data.groups?.map(g => [g.name, badge(g.status), g.studentsCount, g.maxCapacity, progressBar(g.occupancyPercent)]),
        'Нет данных'
      );
      break;
    }
    case 'dir-popularity': {
      const data = await fetchCached('dirPopularity', '/api/Analytics/directions/popularity');
      if (!data) { showErr(el, 'Ошибка загрузки'); return; }
      el.innerHTML = buildTable(
        ['Направление','Групп','Учеников','Тренеров'],
        data.map(d => [d.name, d.groupsCount, d.studentsCount, d.trainersCount]),
        'Нет данных'
      );
      break;
    }
    case 'dir-workload': {
      const data = await fetchCached('dirWorkload', '/api/Analytics/trainers/workload');
      if (!data) { showErr(el, 'Ошибка загрузки'); return; }
      el.innerHTML = buildTable(
        ['Тренер','Групп','Учеников','Проведено','Предстоит'],
        data.map(t => [`${t.firstName} ${t.lastName}`, t.groupsCount, t.studentsCount, t.classesCount, t.upcomingClasses]),
        'Нет данных'
      );
      break;
    }
    case 'acc-summary': {
      const data = await fetchCached('accSummary', '/api/Finance/payments-summary');
      if (!data) { showErr(el, 'Ошибка загрузки'); return; }
      updateAccStats(null, data);
      el.innerHTML = buildTable(
        ['Статус','Количество','Сумма'],
        data.summary?.map(s => [badge(s.status), s.count, fmtMoney(s.totalAmount)]),
        'Нет данных'
      );
      break;
    }
    case 'acc-revenue': {
      const data = await fetchCached('accRevenue', '/api/Finance/revenue');
      if (!data) { showErr(el, 'Ошибка загрузки'); return; }
      updateAccStats(data, null);
      el.innerHTML = `<div class="stats-grid">
        <div class="stat-card accent">
          <div class="stat-val">${fmtMoney(data.totalRevenue)}</div>
          <div class="stat-label">Общая выручка</div>
        </div>
      </div>`;
      break;
    }
    case 'acc-debts': {
      const data = await fetchCached('accDebts', '/api/Finance/debts');
      if (!data) { showErr(el, 'Ошибка загрузки'); return; }
      updateAccStats(null, null, data);
      el.innerHTML = buildTable(
        ['Клиент','Оплачено','Всего','Долг'],
        data.map(c => [
          `${c.lastName} ${c.firstName}`,
          fmtMoney(c.totalPaid), fmtMoney(c.totalAmount),
          `<span style="color:var(--danger);font-weight:600">${fmtMoney(c.debt)}</span>`,
        ]),
        'Задолженностей нет 🎉'
      );
      break;
    }
    case 'admin-clients': {
      const data = await fetchCached('adminClients', '/api/Clients');
      if (!data) { showErr(el, 'Ошибка загрузки'); return; }
      updateAdminStats(data, null, null);
      renderClientsTable(el, data);
      document.getElementById('add-client-btn').style.display = '';
      break;
    }
    case 'admin-trainers': {
      const data = await fetchCached('adminTrainers', '/api/Trainers');
      if (!data) { showErr(el, 'Ошибка загрузки'); return; }
      updateAdminStats(null, data, null);
      el.innerHTML = buildTable(
        ['Фамилия','Имя','Направление','Телефон','Email'],
        data.map(t => [t.lastName, t.firstName, t.direction?.name??'—', t.phone??'—', `<span style="color:var(--muted)">${t.email??'—'}</span>`]),
        'Нет тренеров'
      );
      document.getElementById('add-client-btn').style.display = 'none';
      break;
    }
    case 'admin-groups': {
      const data = await fetchCached('adminGroups', '/api/Groups');
      if (!data) { showErr(el, 'Ошибка загрузки'); return; }
      updateAdminStats(null, null, data);
      el.innerHTML = buildTable(
        ['Название','Направление','Тренер','Статус'],
        data.map(g => [
          g.name, g.direction?.name??'—',
          g.trainer ? `${g.trainer.firstName} ${g.trainer.lastName}` : '—',
          badge(g.status),
        ]),
        'Нет групп'
      );
      document.getElementById('add-client-btn').style.display = 'none';
      break;
    }
    default: break;
  }
}

function updateClientStats(schedule, groups, payments) {
  const el = document.getElementById('client-stats');
  const s  = cache.clientSchedule ?? schedule ?? [];
  const g  = cache.clientGroups   ?? groups   ?? [];
  const p  = cache.clientPayments ?? payments;
  el.innerHTML = buildStats([
    [Array.isArray(s) ? s.length : '—', 'Занятий'],
    [Array.isArray(g) ? g.length : '—', 'Групп'],
    [p ? fmtMoney(p.totalPaid) : '—', 'Оплачено'],
    [p ? fmtMoney(p.totalDebt) : '—', 'К оплате'],
  ]);
}

function updateTrainerStats(schedule, groups) {
  const el = document.getElementById('trainer-stats');
  const s  = cache.trainerSchedule ?? schedule ?? [];
  const g  = cache.trainerGroups   ?? groups   ?? [];
  const total = Array.isArray(g) ? g.reduce((a, x) => a + (x.studentsCount ?? 0), 0) : '—';
  el.innerHTML = buildStats([
    [Array.isArray(s) ? s.length : '—', 'Занятий'],
    [Array.isArray(g) ? g.length : '—', 'Групп'],
    [total, 'Учеников'],
  ]);
}

function updateAccStats(revenue, summary, debts) {
  const el = document.getElementById('acc-stats');
  const r  = cache.accRevenue ?? revenue;
  const s  = cache.accSummary ?? summary;
  const d  = cache.accDebts   ?? debts ?? [];
  el.innerHTML = buildStats([
    [r ? fmtMoney(r.totalRevenue) : '—', 'Выручка', true],
    [s ? s.totalSubscriptions : '—', 'Абонементов'],
    [Array.isArray(d) ? d.length : '—', 'Должников'],
  ]);
}

function updateAdminStats(clients, trainers, groups) {
  const el = document.getElementById('admin-stats');
  const c  = cache.adminClients  ?? clients  ?? [];
  const t  = cache.adminTrainers ?? trainers ?? [];
  const g  = cache.adminGroups   ?? groups   ?? [];
  el.innerHTML = buildStats([
    [Array.isArray(c) ? c.length : '—', 'Клиентов'],
    [Array.isArray(t) ? t.length : '—', 'Тренеров'],
    [Array.isArray(g) ? g.length : '—', 'Групп'],
  ]);
}

function buildTrainerGroupsTable(groups) {
  if (!groups?.length) return `<div class="empty-state"><div class="empty-icon">🌿</div>Нет групп</div>`;
  let rows = '';
  groups.forEach((g, i) => {
    rows += `<tr>
      <td><strong>${g.name}</strong></td>
      <td>${g.directionName ?? '—'}</td>
      <td>${badge(g.status)}</td>
      <td>${g.studentsCount}</td>
      <td>
        <button class="btn btn-ghost btn-sm expand-btn" data-idx="${i}" data-expanded="0">▼ Ученики</button>
      </td>
    </tr>`;
    if (g.students?.length) {
      g.students.forEach(s => {
        rows += `<tr class="sub-row sub-rows-${i}" style="display:none">
          <td colspan="2" style="padding-left:32px">👤 ${s.lastName} ${s.firstName}</td>
          <td style="color:var(--muted)">${s.phone ?? '—'}</td>
          <td colspan="2" style="color:var(--muted)">${s.email ?? '—'}</td>
        </tr>`;
      });
    }
  });
  return `<div class="table-wrap"><table>
    <thead><tr>
      <th>Группа</th><th>Направление</th><th>Статус</th><th>Учеников</th><th></th>
    </tr></thead>
    <tbody>${rows}</tbody>
  </table></div>`;
}

function renderClientsTable(el, data) {
  if (!data?.length) {
    el.innerHTML = `<div class="empty-state"><div class="empty-icon">🌿</div>Нет клиентов</div>`;
    return;
  }
  const rows = data.map(c => {
    const id = c.client_id ?? c.clientId ?? '';
    return `<tr>
      <td>${c.lastName ?? '—'}</td>
      <td>${c.firstName ?? '—'}</td>
      <td>${c.age ?? '—'}</td>
      <td>${c.phone ?? '—'}</td>
      <td style="color:var(--muted)">${c.email ?? '—'}</td>
      <td><button class="btn btn-danger btn-sm del-client-btn" data-id="${id}">Удалить</button></td>
    </tr>`;
  }).join('');
  el.innerHTML = `<div class="table-wrap"><table>
    <thead><tr>
      <th>Фамилия</th><th>Имя</th><th>Возраст</th><th>Телефон</th><th>Email</th><th></th>
    </tr></thead>
    <tbody>${rows}</tbody>
  </table></div>`;
  el.querySelectorAll('.del-client-btn').forEach(btn => {
    btn.addEventListener('click', async () => {
      const id = btn.dataset.id;
      if (!confirm('Удалить клиента?')) return;
      const { ok } = await apiFetch(`/api/Clients/${id}`, { method: 'DELETE' });
      if (ok) {
        cache.adminClients = cache.adminClients?.filter(c => String(c.client_id ?? c.clientId) !== String(id));
        renderClientsTable(el, cache.adminClients);
        updateAdminStats(cache.adminClients, null, null);
      } else {
        alert('Нельзя удалить — есть связанные данные');
      }
    });
  });
}

const modal      = document.getElementById('add-client-modal');
const modalError = document.getElementById('modal-error');

document.getElementById('add-client-btn').addEventListener('click', () => {
  clearModalForm();
  modal.classList.add('open');
});
document.getElementById('modal-cancel').addEventListener('click', () => modal.classList.remove('open'));
modal.addEventListener('click', e => { if (e.target === modal) modal.classList.remove('open'); });

document.getElementById('modal-save').addEventListener('click', async () => {
  const lastName  = document.getElementById('f-lastName').value.trim();
  const firstName = document.getElementById('f-firstName').value.trim();
  const age       = document.getElementById('f-age').value;
  const phone     = document.getElementById('f-phone').value.trim();
  const email     = document.getElementById('f-email').value.trim();
  modalError.style.display = 'none';
  if (!lastName || !firstName) {
    modalError.textContent = 'Заполните фамилию и имя';
    modalError.style.display = 'block';
    return;
  }
  const saveBtn = document.getElementById('modal-save');
  saveBtn.textContent = 'Сохранение…';
  saveBtn.disabled = true;
  const { ok, data } = await apiFetch('/api/Clients', {
    method: 'POST',
    body: JSON.stringify({ lastName, firstName, age: age ? parseInt(age) : null, phone, email }),
  });
  saveBtn.textContent = 'Сохранить';
  saveBtn.disabled = false;
  if (ok) {
    modal.classList.remove('open');
    delete cache.adminClients;
    const pane = document.getElementById('admin-clients');
    delete pane.dataset.loaded;
    loadPane('admin-clients');
  } else {
    modalError.textContent = getField(data, 'message') ?? 'Ошибка сохранения';
    modalError.style.display = 'block';
  }
});

function clearModalForm() {
  ['f-lastName','f-firstName','f-age','f-phone','f-email'].forEach(id => {
    document.getElementById(id).value = '';
  });
  modalError.style.display = 'none';
}

// Initial load
if (ROLE === 'Trainer') {
  loadPane('trainer-schedule');
  loadPane('trainer-groups');
}
else if (ROLE === 'Client') {
  loadPane('client-schedule');
  loadPane('client-groups');
  loadPane('client-payments');
}
else if (ROLE === 'Director') {
  loadPane('dir-occupancy');
  loadPane('dir-popularity');
  loadPane('dir-workload');
}
else if (ROLE === 'Accountant') {
  loadPane('acc-summary');
  loadPane('acc-revenue');
  loadPane('acc-debts');
}
else if (ROLE === 'Admin') {
  loadPane('admin-clients');
  loadPane('admin-trainers');
  loadPane('admin-groups');
}