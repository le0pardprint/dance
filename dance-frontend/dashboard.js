if (!getToken() || !getRole()) {
  window.location.href = 'login.html';
}

const ROLE  = getRole();
const EMAIL = getEmail();

const ROLE_LABELS = {
  Client: 'Клиент', Trainer: 'Тренер',
  Director: 'Директор', Accountant: 'Бухгалтер', Admin: 'Администратор',
};
const PAGE_TITLES = {
  Client: 'Личный кабинет', Trainer: 'Кабинет тренера',
  Director: 'Аналитика', Accountant: 'Финансы', Admin: 'Управление',
};

document.getElementById('api-label').textContent  = API_URL;
document.getElementById('page-title').textContent = PAGE_TITLES[ROLE] ?? 'Главная';
document.getElementById('sb-email').textContent   = EMAIL ?? '—';
document.getElementById('sb-role').textContent    = ROLE_LABELS[ROLE] ?? ROLE;
document.getElementById('sb-avatar').textContent  = EMAIL ? EMAIL[0].toUpperCase() : '?';

const navEl = document.getElementById('sidebar-nav');
const navItem = document.createElement('div');
navItem.className = 'nav-item active';
navItem.textContent = PAGE_TITLES[ROLE] ?? 'Главная';
navEl.appendChild(navItem);

document.querySelectorAll('.page').forEach(p => p.classList.remove('active'));
const rolePage = document.getElementById(`page-${ROLE.toLowerCase()}`);
if (rolePage) rolePage.classList.add('active');

document.getElementById('logout-btn').addEventListener('click', () => {
  clearAuth(); window.location.href = 'login.html';
});

// Tabs
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
        if (ROLE === 'Admin') {
          const addGroupBtn   = document.getElementById('add-group-btn');
          const addTrainerBtn = document.getElementById('add-trainer-btn');
          const addClassBtn   = document.getElementById('add-class-btn');
          if (addGroupBtn)   addGroupBtn.style.display   = paneId === 'admin-groups'   ? '' : 'none';
          if (addTrainerBtn) addTrainerBtn.style.display  = paneId === 'admin-trainers' ? '' : 'none';
          if (addClassBtn)   addClassBtn.style.display    = paneId === 'admin-groups'   ? '' : 'none';
        }
      }
    });
  });
});

// Cache
const cache = {};
async function fetchCached(key, url) {
  if (cache[key]) return cache[key];
  const { ok, data } = await apiFetch(url);
  if (ok) cache[key] = data;
  return ok ? data : null;
}

// Pane loader
async function loadPane(paneId) {
  const el = document.getElementById(paneId);
  if (!el) return;
  el.dataset.loaded = '1';
  showLoading(el);

  switch (paneId) {

    // CLIENT
        case 'client-schedule': {
            const profile = await fetchCached('clientProfile', '/api/Client/profile');
            const banner = document.getElementById('client-name-banner');
            if (banner && profile) {
              banner.innerHTML = `<div class="client-hello">Добро пожаловать, <strong>${profile.firstName} ${profile.lastName}</strong></div>`;
            }
            const data = await fetchCached('clientSchedule', '/api/Client/schedule');
            if (!data) { showErr(el, 'Не удалось загрузить расписание'); return; }
            updateClientStats(data, null, null);
            el.innerHTML = buildTable(
              ['Дата','Время','Группа','Направление','Тренер','Статус',''],
              data.map(c => [
                fmtDate(c.date), c.time??'—', c.groupName??'—',
                c.directionName??'—', c.trainerName??'—', badge(c.status),
                c.status === 'Запланировано'
                  ? `<button class="btn btn-danger btn-sm cancel-class-btn" data-id="${c.class_id}">Отменить</button>`
                  : '—'
              ]),
              'Расписание пусто'
            );
            el.querySelectorAll('.cancel-class-btn').forEach(btn => {
              btn.addEventListener('click', async () => {
                if (!confirm('Отменить запись на это занятие?')) return;
                alert('Функция отмены записи: обратитесь к администратору');
              });
            });
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

    // TRAINER
    case 'trainer-schedule': {
      const data = await fetchCached('trainerSchedule', '/api/Trainer/schedule');
      if (!data) { showErr(el, 'Не удалось загрузить расписание'); return; }
      updateTrainerStats(data, null);
      el.innerHTML = buildTable(
        ['Дата','Время','Группа','Учеников','Статус',''],
        data.map(c => [
          fmtDate(c.date), c.time??'—', c.groupName??'—',
          c.studentsCount??'—', badge(c.status),
          `<button class="btn btn-ghost btn-sm change-status-btn"
            data-id="${c.class_id}" data-status="${c.status}">Изменить статус</button>`
        ]),
        'Расписание пусто'
      );
      el.querySelectorAll('.change-status-btn').forEach(btn => {
        btn.addEventListener('click', () => {
          document.getElementById('sm-class-id').value = btn.dataset.id;
          document.getElementById('sm-status').value   = btn.dataset.status;
          document.getElementById('status-modal').classList.add('open');
        });
      });
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
          btn.textContent = expanded ? 'Ученики' : 'Скрыть';
        });
      });
      break;
    }

    // DIRECTOR
    case 'dir-occupancy': {
      const data = await fetchCached('dirOccupancy', '/api/Analytics/groups/occupancy');
      if (!data) { showErr(el, 'Ошибка загрузки'); return; }
      document.getElementById('director-stats').innerHTML = buildStats([
        [data.totalGroups, 'Групп'],
        [data.totalStudents, 'Учеников'],
        [data.averageOccupancyPercent + '%', 'Заполненность'],
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
      el.innerHTML = buildDirectorPopularityTable(data);
      el.querySelectorAll('.expand-trainers-btn').forEach(btn => {
        btn.addEventListener('click', () => {
          const idx      = btn.dataset.idx;
          const subRows  = el.querySelectorAll(`.trainer-rows-${idx}`);
          const expanded = btn.dataset.expanded === '1';
          subRows.forEach(r => r.style.display = expanded ? 'none' : '');
          btn.dataset.expanded = expanded ? '0' : '1';
          btn.textContent = expanded ? 'Тренеры' : 'Скрыть';
        });
      });
      break;
    }
    case 'dir-workload': {
      const data = await fetchCached('dirWorkload', '/api/Analytics/trainers/workload');
      if (!data) { showErr(el, 'Ошибка загрузки'); return; }
      el.innerHTML = buildTable(
        ['Тренер','Направление','Групп','Учеников','Проведено','Предстоит'],
        data.map(t => [
          `${t.firstName} ${t.lastName}`,
          t.directionName ?? '—',
          t.groupsCount, t.studentsCount, t.classesCount, t.upcomingClasses
        ]),
        'Нет данных'
      );
      break;
    }

    // ACCOUNTANT
  case 'acc-summary': {
  const data = await fetchCached('accSummary', '/api/Finance/payments-summary');
  const clients = await fetchCached('adminClients', '/api/Clients');
  if (!data) { showErr(el, 'Ошибка загрузки'); return; }
  updateAccStats(null, data);

  const paid   = data.details?.filter(s => s.status === 'Оплачен')  ?? [];
  const active = data.details?.filter(s => s.status === 'Активен')  ?? [];
  const other  = data.details?.filter(s => s.status !== 'Оплачен' && s.status !== 'Активен') ?? [];

  // Клиенты без абонементов
  const clientsWithSubs = new Set(data.details?.map(s => s.clientName) ?? []);
  const noSubs = (clients ?? []).filter(c =>
    !clientsWithSubs.has(`${c.lastName} ${c.firstName}`)
  );

  el.innerHTML = `
    <h4 style="margin-bottom:12px;font-size:14px;color:var(--muted)">Оплаченные</h4>
    ${buildTable(['Client ID','Клиент','Группа','Сумма','Статус'],
      paid.map(s => [s.client_id ?? '—', s.clientName, s.groupName, fmtMoney(s.amount), badge(s.status)]), 'Нет')}
    <h4 style="margin:16px 0 12px;font-size:14px;color:var(--muted)">Активные</h4>
    ${buildTable(['Client ID','Клиент','Группа','Сумма','Статус'],
      active.map(s => [s.client_id ?? '—', s.clientName, s.groupName, fmtMoney(s.amount), badge(s.status)]), 'Нет')}
    ${other.length ? `<h4 style="margin:16px 0 12px;font-size:14px;color:var(--muted)">Прочие</h4>
    ${buildTable(['Client ID','Клиент','Группа','Сумма','Статус'],
      other.map(s => [s.client_id ?? '—', s.clientName, s.groupName, fmtMoney(s.amount), badge(s.status)]), 'Нет')}` : ''}
    ${noSubs.length ? `<h4 style="margin:16px 0 12px;font-size:14px;color:var(--muted)">Без абонементов</h4>
    ${buildTable(['Client ID','Клиент','Email',''],
      noSubs.map(c => [c.client_id ?? '—', `${c.lastName} ${c.firstName}`, c.email ?? '—',
        `<button class="btn btn-ghost btn-sm add-sub-btn" data-id="${c.client_id}">+ Абонемент</button>`
      ]), 'Нет')}` : ''}
  `;
  el.querySelectorAll('.add-sub-btn').forEach(btn => {
  btn.addEventListener('click', async () => {
    document.getElementById('ad-clientId').value = btn.dataset.id;
    // Загружаем группы
    const groups = await fetchCached('accGroups', '/api/Groups');
    const grpSelect = document.getElementById('ad-group');
    if (groups && grpSelect) {
      grpSelect.innerHTML = groups.map(g => `<option value="${g.group_id ?? g.groupId}">${g.name}</option>`).join('');
    }
    document.getElementById('add-debt-modal').classList.add('open');
  });
});
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
      if (!data.length) { el.innerHTML = `<div class="empty-state">Задолженностей нет</div>`; return; }
      el.innerHTML = buildTable(
        ['Клиент','Оплачено','Всего','Долг',''],
        data.map(c => [
          `${c.lastName} ${c.firstName}`,
          fmtMoney(c.totalPaid), fmtMoney(c.totalAmount),
          `<span style="color:var(--danger);font-weight:600">${fmtMoney(c.debt)}</span>`,
          `<button class="btn btn-ghost btn-sm edit-debt-btn" data-id="${c.client_id ?? c.clientId}">Изменить</button>`
        ]),
        'Задолженностей нет'
      );
      el.querySelectorAll('.edit-debt-btn').forEach(btn => {
        btn.addEventListener('click', async () => {
          const clientId = btn.dataset.id;
          const { ok, data: subs } = await apiFetch(`/api/Subscriptions/byclient/${clientId}`);
          if (!ok || !subs?.length) { alert('Нет абонементов'); return; }
          const sub = subs.find(s => s.status === 'Активен') ?? subs[0];
          const subId = sub.sub_id ?? sub.subId;
          document.getElementById('dm-sub-id').value = subId;
          document.getElementById('dm-status').value = sub.status ?? 'Активен';
          document.getElementById('debt-modal').classList.add('open');
        });
      });
      break;
    }

    // ADMIN
    case 'admin-clients': {
      const data = await fetchCached('adminClients', '/api/Clients');
      if (!data) { showErr(el, 'Ошибка загрузки'); return; }
      updateAdminStats(data, null, null);
      renderClientsTable(el, data);
      document.getElementById('add-client-btn').style.display = '';
      document.getElementById('add-user-btn').style.display   = '';
      const addGroupBtn = document.getElementById('add-group-btn');
      if (addGroupBtn) addGroupBtn.style.display = 'none';
      break;
    }
    case 'admin-trainers': {
      const data = await fetchCached('adminTrainers', '/api/Trainers');
      if (!data) { showErr(el, 'Ошибка загрузки'); return; }
      updateAdminStats(null, data, null);
      el.innerHTML = buildAdminTrainersTable(data);
      el.querySelectorAll('.expand-trainer-btn').forEach(btn => {
        btn.addEventListener('click', () => {
          const idx      = btn.dataset.idx;
          const subRows  = el.querySelectorAll(`.trainer-detail-${idx}`);
          const expanded = btn.dataset.expanded === '1';
          subRows.forEach(r => r.style.display = expanded ? 'none' : '');
          btn.dataset.expanded = expanded ? '0' : '1';
          btn.textContent = expanded ? 'Расписание' : 'Скрыть';
        });
      });
      const addTrainerBtn = document.getElementById('add-trainer-btn');
      if (addTrainerBtn) addTrainerBtn.style.display = '';
      const addGroupBtn2 = document.getElementById('add-group-btn');
      if (addGroupBtn2) addGroupBtn2.style.display = 'none';
      break;
    }
    case 'admin-groups': {
  const data = await fetchCached('adminGroups', '/api/Groups');
  if (!data) { showErr(el, 'Ошибка загрузки'); return; }
  updateAdminStats(null, null, data);
  el.innerHTML = buildAdminGroupsTable(data);
  el.querySelectorAll('.expand-group-btn').forEach(btn => {
    btn.addEventListener('click', async () => {
      const groupId  = btn.dataset.id;
      const idx      = btn.dataset.idx;
      const subRows  = el.querySelectorAll(`.group-members-${idx}`);
      const expanded = btn.dataset.expanded === '1';
      if (!expanded && subRows.length === 0) {
        const { ok, data: regs } = await apiFetch(`/api/Registration/bygroup/${groupId}`);
        if (ok && regs?.length) {
          regs.forEach(r => {
            const row = document.createElement('tr');
            row.className = `sub-row group-members-${idx}`;
            row.innerHTML = `
              <td colspan="2" style="padding-left:32px">
                ${r.client?.lastName ?? ''} ${r.client?.firstName ?? ''}
              </td>
              <td style="color:var(--muted)">${r.client?.phone ?? '—'}</td>
              <td colspan="3"></td>`;
            btn.closest('tr').after(row);
          });
        }
      } else {
        subRows.forEach(r => r.style.display = expanded ? 'none' : '');
      }
      btn.dataset.expanded = expanded ? '0' : '1';
      btn.textContent = expanded ? 'Состав' : 'Скрыть';
    });
  });
  el.querySelectorAll('.change-group-status-btn').forEach(btn => {
    btn.addEventListener('click', () => {
      document.getElementById('gs-group-id').value = btn.dataset.id;
      document.getElementById('gs-status').value   = btn.dataset.status;
      document.getElementById('group-status-modal').classList.add('open');
    });
  });
  el.querySelectorAll('.del-group-btn').forEach(btn => {
    btn.addEventListener('click', async () => {
      const id = btn.dataset.id;
      if (!confirm('Удалить группу? Все связанные данные будут удалены.')) return;
      const { ok } = await apiFetch(`/api/Groups/${id}`, { method: 'DELETE' });
      if (ok) {
        delete cache.adminGroups;
        const pane = document.getElementById('admin-groups');
        delete pane.dataset.loaded;
        loadPane('admin-groups');
      } else {
        alert('Ошибка при удалении группы');
      }
    });
  });
  const addGroupBtn3 = document.getElementById('add-group-btn');
  if (addGroupBtn3) addGroupBtn3.style.display = '';
  const addClassBtn = document.getElementById('add-class-btn');
  if (addClassBtn) addClassBtn.style.display = '';
  break;
}

case 'admin-schedule': {
  const data = await fetchCached('adminSchedule', '/api/Classes');
  if (!data) { showErr(el, 'Ошибка загрузки'); return; }
  el.innerHTML = buildTable(
    ['Дата','Время','Группа','Тренер','Статус',''],
    data.map(c => [
      fmtDate(c.date),
      c.time ?? '—',
      c.group?.name ?? '—',
      c.trainer ? `${c.trainer.firstName} ${c.trainer.lastName}` : '—',
      badge(c.status),
      `<button class="btn btn-ghost btn-sm change-class-status-btn" data-id="${c.class_id}" data-status="${c.status}">Статус</button>`
    ]),
    'Расписание пусто'
  );
  el.querySelectorAll('.change-class-status-btn').forEach(btn => {
    btn.addEventListener('click', () => {
      document.getElementById('sm-class-id').value = btn.dataset.id;
      document.getElementById('sm-status').value   = btn.dataset.status;
      document.getElementById('status-modal').classList.add('open');
    });
  });
  const addGroupBtn = document.getElementById('add-group-btn');
  if (addGroupBtn) addGroupBtn.style.display = 'none';
  const addTrainerBtn = document.getElementById('add-trainer-btn');
  if (addTrainerBtn) addTrainerBtn.style.display = 'none';
  const addClassBtn = document.getElementById('add-class-btn');
  if (addClassBtn) addClassBtn.style.display = 'none';
  break;
}

    default: break;
  }
}

// Stats updaters
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
  const el    = document.getElementById('trainer-stats');
  const s     = cache.trainerSchedule ?? schedule ?? [];
  const g     = cache.trainerGroups   ?? groups   ?? [];
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

// Table builders
function buildTrainerGroupsTable(groups) {
  if (!groups?.length) return `<div class="empty-state">Нет групп</div>`;
  let rows = '';
  groups.forEach((g, i) => {
    rows += `<tr>
      <td><strong>${g.name}</strong></td>
      <td>${g.directionName ?? '—'}</td>
      <td>${badge(g.status)}</td>
      <td>${g.studentsCount}</td>
      <td><button class="btn btn-ghost btn-sm expand-btn" data-idx="${i}" data-expanded="0">Ученики</button></td>
    </tr>`;
    if (g.students?.length) {
      g.students.forEach(s => {
        rows += `<tr class="sub-row sub-rows-${i}" style="display:none">
          <td colspan="2" style="padding-left:32px">${s.lastName} ${s.firstName}</td>
          <td style="color:var(--muted)">${s.phone ?? '—'}</td>
          <td colspan="2" style="color:var(--muted)">${s.email ?? '—'}</td>
        </tr>`;
      });
    }
  });
  return `<div class="table-wrap"><table>
    <thead><tr><th>Группа</th><th>Направление</th><th>Статус</th><th>Учеников</th><th></th></tr></thead>
    <tbody>${rows}</tbody>
  </table></div>`;
}

function buildDirectorPopularityTable(data) {
  if (!data?.length) return `<div class="empty-state">Нет данных</div>`;
  let rows = '';
  data.forEach((d, i) => {
    rows += `<tr>
      <td>${d.name}</td>
      <td>${d.groupsCount}</td>
      <td>${d.studentsCount}</td>
      <td>${d.trainersCount}</td>
      <td><button class="btn btn-ghost btn-sm expand-trainers-btn" data-idx="${i}" data-id="${d.direction_id}" data-expanded="0">Тренеры</button></td>
    </tr>`;
    if (d.trainerNames?.length) {
      d.trainerNames.forEach(name => {
        rows += `<tr class="sub-row trainer-rows-${i}" style="display:none">
          <td colspan="4" style="padding-left:32px;color:var(--muted)">${name}</td>
          <td></td>
        </tr>`;
      });
    }
  });
  return `<div class="table-wrap"><table>
    <thead><tr><th>Направление</th><th>Групп</th><th>Учеников</th><th>Тренеров</th><th></th></tr></thead>
    <tbody>${rows}</tbody>
  </table></div>`;
}

function buildAdminTrainersTable(data) {
  if (!data?.length) return `<div class="empty-state">Нет тренеров</div>`;
  let rows = '';
  data.forEach((t, i) => {
    const userId = t.user_id ?? t.userId ?? t.user?.user_id ?? '—';
    const userEmail = t.user?.email ?? '—';
    rows += `<tr>
      <td>${t.lastName}</td>
      <td>${t.firstName}</td>
      <td>${t.direction?.name ?? '—'}</td>
      <td>${t.phone ?? '—'}</td>
      <td style="color:var(--muted)">${t.email ?? '—'}</td>
      <td style="color:var(--muted);font-size:12px">Trainer ID: ${t.trainer_id ?? t.trainerId ?? '—'} | User ID: ${userId}</td>
      <td><button class="btn btn-ghost btn-sm expand-trainer-btn" data-idx="${i}" data-expanded="0">Расписание</button></td>
    </tr>`;
    if (t.classes?.length) {
      t.classes.slice(0,5).forEach(c => {
        rows += `<tr class="sub-row trainer-detail-${i}" style="display:none">
          <td colspan="2" style="padding-left:32px;font-size:12px">${fmtDate(c.date)} ${c.time ?? ''}</td>
          <td style="font-size:12px;color:var(--muted)">${c.status ?? '—'}</td>
          <td colspan="4"></td>
        </tr>`;
      });
    }
  });
  return `<div class="table-wrap"><table>
    <thead><tr><th>Фамилия</th><th>Имя</th><th>Направление</th><th>Телефон</th><th>Email</th><th>IDs</th><th></th></tr></thead>
    <tbody>${rows}</tbody>
  </table></div>`;
}

function buildAdminGroupsTable(data) {
  if (!data?.length) return `<div class="empty-state">Нет групп</div>`;
  let rows = '';
  data.forEach((g, i) => {
    const gid = g.group_id ?? g.groupId ?? '';
    rows += `<tr>
      <td>${g.name}</td>
      <td>${g.direction?.name ?? '—'}</td>
      <td>${g.trainer ? `${g.trainer.firstName} ${g.trainer.lastName}` : '—'}</td>
      <td>${badge(g.status)}</td>
      <td>
        <button class="btn btn-ghost btn-sm change-group-status-btn" data-id="${gid}" data-status="${g.status}">Статус</button>
        <button class="btn btn-ghost btn-sm expand-group-btn" data-idx="${i}" data-id="${gid}" data-expanded="0">Состав</button>
        <button class="btn btn-danger btn-sm del-group-btn" data-id="${gid}">Удалить</button>
      </td>
    </tr>`;
  });
  return `<div class="table-wrap"><table>
    <thead><tr><th>Название</th><th>Направление</th><th>Тренер</th><th>Статус</th><th></th></tr></thead>
    <tbody>${rows}</tbody>
  </table></div>`;
}

function renderClientsTable(el, data) {
  if (!data?.length) {
    el.innerHTML = `<div class="empty-state">Нет клиентов</div>`;
    return;
  }
  const rows = data.map(c => {
    const id = c.client_id ?? c.clientId ?? '';
    const userId = c.user_id ?? c.userId ?? '—';
    return `<tr>
      <td>${c.lastName ?? '—'}</td>
      <td>${c.firstName ?? '—'}</td>
      <td>${c.age ?? '—'}</td>
      <td>${c.phone ?? '—'}</td>
      <td style="color:var(--muted)">${c.email ?? '—'}</td>
      <td style="color:var(--muted);font-size:12px">Client ID: ${id} | User ID: ${userId}</td>
      <td>
        <button class="btn btn-ghost btn-sm edit-client-btn" data-id="${id}"
          data-ln="${c.lastName??''}" data-fn="${c.firstName??''}"
          data-age="${c.age??''}" data-phone="${c.phone??''}" data-email="${c.email??''}">
          Изменить
        </button>
        <button class="btn btn-ghost btn-sm reg-client-btn" data-id="${id}" data-name="${c.lastName??''} ${c.firstName??''}">В группу</button>
        <button class="btn btn-danger btn-sm del-client-btn" data-id="${id}">Удалить</button>
      </td>
    </tr>`;
  }).join('');

  el.innerHTML = `<div class="table-wrap"><table>
    <thead><tr><th>Фамилия</th><th>Имя</th><th>Возраст</th><th>Телефон</th><th>Email</th><th>IDs</th><th></th></tr></thead>
    <tbody>${rows}</tbody>
  </table></div>`;

  el.querySelectorAll('.edit-client-btn').forEach(btn => {
  btn.addEventListener('click', async () => {
    document.getElementById('ef-id').value        = btn.dataset.id;
    document.getElementById('ef-lastName').value  = btn.dataset.ln;
    document.getElementById('ef-firstName').value = btn.dataset.fn;
    document.getElementById('ef-age').value       = btn.dataset.age;
    document.getElementById('ef-phone').value     = btn.dataset.phone;
    document.getElementById('ef-email').value     = btn.dataset.email;
    document.getElementById('ef-amount').value    = '';
    // Загружаем группы
    const groups = await fetchCached('adminGroups', '/api/Groups');
    const grpSelect = document.getElementById('ef-group');
    if (groups && grpSelect) {
      grpSelect.innerHTML = '<option value="">— Без группы —</option>' +
        groups.map(g => `<option value="${g.group_id ?? g.groupId}">${g.name}</option>`).join('');
    }
    document.getElementById('edit-client-modal').classList.add('open');
  });
});

  el.querySelectorAll('.del-client-btn').forEach(btn => {
    btn.addEventListener('click', async () => {
      const id = btn.dataset.id;
      if (!confirm('Удалить клиента? Все связанные данные (регистрации, абонементы, посещения) будут удалены.')) return;
      const { ok } = await apiFetch(`/api/Clients/${id}`, { method: 'DELETE' });
      if (ok) {
        cache.adminClients = cache.adminClients?.filter(c => String(c.client_id ?? c.clientId) !== String(id));
        renderClientsTable(el, cache.adminClients);
        updateAdminStats(cache.adminClients, null, null);
      } else {
        alert('Ошибка при удалении клиента');
      }
    });
  });
    el.querySelectorAll('.reg-client-btn').forEach(btn => {
      btn.addEventListener('click', async () => {
        document.getElementById('reg-client-id').value = btn.dataset.id;
        document.getElementById('reg-client-name').textContent = btn.dataset.name;
        document.getElementById('reg-amount').value = '';
        regClientModalError.style.display = 'none';

        const groups = await fetchCached('adminGroups', '/api/Groups');
        const grpSelect = document.getElementById('reg-group');
        if (groups && grpSelect) {
          grpSelect.innerHTML = groups.map(g => `<option value="${g.group_id ?? g.groupId}">${g.name}</option>`).join('');
        }

        // Загружаем занятия для первой группы
        await loadClassesForGroup(grpSelect.value);

        // При смене группы — обновляем занятия
        grpSelect.onchange = async () => await loadClassesForGroup(grpSelect.value);

        regClientModal.classList.add('open');
    });
  });
}

// Modal: добавить клиента
const modal      = document.getElementById('add-client-modal');
const modalError = document.getElementById('modal-error');

document.getElementById('add-client-btn').addEventListener('click', async () => {
  clearModalForm();
  // Загружаем группы для селекта
  const groups = await fetchCached('adminGroups', '/api/Groups');
  const grpSelect = document.getElementById('f-group');
  if (groups && grpSelect) {
    grpSelect.innerHTML = '<option value="">— Без группы —</option>' +
      groups.map(g => `<option value="${g.group_id ?? g.groupId}">${g.name}</option>`).join('');
  }
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
  const groupId   = document.getElementById('f-group').value;
  const amount    = document.getElementById('f-amount').value;
  modalError.style.display = 'none';
  if (!lastName || !firstName) {
    modalError.textContent = 'Заполните фамилию и имя';
    modalError.style.display = 'block'; return;
  }
const saveBtn = document.getElementById('modal-save');
  saveBtn.textContent = 'Сохранение…'; saveBtn.disabled = true;
  const { ok, data } = await apiFetch('/api/Clients', {
    method: 'POST',
    body: JSON.stringify({ lastName, firstName, age: age ? parseInt(age) : null, phone, email }),
  });
  if (!ok) {
    saveBtn.textContent = 'Сохранить'; saveBtn.disabled = false;
    modalError.textContent = getField(data, 'message') ?? 'Ошибка сохранения';
    modalError.style.display = 'block'; return;
  }
  const clientId = data?.client_id ?? data?.clientId;
  // Если выбрана группа — добавляем регистрацию и абонемент
  if (groupId && clientId) {
    await apiFetch('/api/Registration', {
      method: 'POST',
      body: JSON.stringify({ client_id: clientId, group_id: parseInt(groupId), registration_date: new Date().toISOString() }),
    });
    if (amount) {
      await apiFetch('/api/Subscriptions', {
        method: 'POST',
        body: JSON.stringify({ client_id: clientId, group_id: parseInt(groupId), amount: parseFloat(amount), status: 'Активен' }),
      });
    }
  }
  saveBtn.textContent = 'Сохранить'; saveBtn.disabled = false;
  modal.classList.remove('open');
  delete cache.adminClients;
  const pane = document.getElementById('admin-clients');
  delete pane.dataset.loaded;
  loadPane('admin-clients');
});

function clearModalForm() {
  ['f-lastName','f-firstName','f-age','f-phone','f-email','f-amount'].forEach(id => {
    document.getElementById(id).value = '';
  });
  modalError.style.display = 'none';
}

// Modal: редактировать клиента
const editModal      = document.getElementById('edit-client-modal');
const editModalError = document.getElementById('edit-modal-error');

document.getElementById('edit-modal-cancel').addEventListener('click', () => editModal.classList.remove('open'));
editModal.addEventListener('click', e => { if (e.target === editModal) editModal.classList.remove('open'); });

document.getElementById('edit-modal-save').addEventListener('click', async () => {
  const id        = document.getElementById('ef-id').value;
  const lastName  = document.getElementById('ef-lastName').value.trim();
  const firstName = document.getElementById('ef-firstName').value.trim();
  const age       = document.getElementById('ef-age').value;
  const phone     = document.getElementById('ef-phone').value.trim();
  const email     = document.getElementById('ef-email').value.trim();
  editModalError.style.display = 'none';
  if (!lastName || !firstName) {
    editModalError.textContent = 'Заполните фамилию и имя';
    editModalError.style.display = 'block'; return;
  }
  const saveBtn = document.getElementById('edit-modal-save');
  saveBtn.textContent = 'Сохранение…'; saveBtn.disabled = true;
  const { ok, data } = await apiFetch(`/api/Clients/${id}`, {
    method: 'PUT',
    body: JSON.stringify({ client_id: parseInt(id), lastName, firstName, age: age ? parseInt(age) : null, phone, email }),
  });
  saveBtn.textContent = 'Сохранить'; saveBtn.disabled = false;
  if (ok) {
  // Если выбрана группа — добавляем регистрацию и абонемент
  const groupId = document.getElementById('ef-group').value;
  const amount  = document.getElementById('ef-amount').value;
  if (groupId) {
    await apiFetch('/api/Registration', {
      method: 'POST',
      body: JSON.stringify({ client_id: parseInt(id), group_id: parseInt(groupId), registration_date: new Date().toISOString() }),
    });
    if (amount) {
      await apiFetch('/api/Subscriptions', {
        method: 'POST',
        body: JSON.stringify({ client_id: parseInt(id), group_id: parseInt(groupId), amount: parseFloat(amount), status: 'Активен' }),
      });
    }
  }
  editModal.classList.remove('open');
  delete cache.adminClients;
  const pane = document.getElementById('admin-clients');
  delete pane.dataset.loaded;
  loadPane('admin-clients');
}
});

// Modal: добавить пользователя
const userModal      = document.getElementById('add-user-modal');
const userModalError = document.getElementById('user-modal-error');

document.getElementById('add-user-btn').addEventListener('click', () => {
  userModalError.style.display = 'none';
  ['u-email','u-password','u-clientId','u-trainerId'].forEach(id => document.getElementById(id).value = '');
  userModal.classList.add('open');
});
document.getElementById('user-modal-cancel').addEventListener('click', () => userModal.classList.remove('open'));
userModal.addEventListener('click', e => { if (e.target === userModal) userModal.classList.remove('open'); });

document.getElementById('user-modal-save').addEventListener('click', async () => {
  const email     = document.getElementById('u-email').value.trim();
  const password  = document.getElementById('u-password').value;
  const role      = document.getElementById('u-role').value;
  const clientId  = document.getElementById('u-clientId').value;
  const trainerId = document.getElementById('u-trainerId').value;
  userModalError.style.display = 'none';
  if (!email || !password) {
    userModalError.textContent = 'Заполните email и пароль';
    userModalError.style.display = 'block'; return;
  }
  const saveBtn = document.getElementById('user-modal-save');
  saveBtn.textContent = 'Создание…'; saveBtn.disabled = true;
  const { ok, data } = await apiFetch('/api/Auth/register', {
    method: 'POST',
    body: JSON.stringify({
      email, password, role,
      client_id:  clientId  ? parseInt(clientId)  : null,
      trainer_id: trainerId ? parseInt(trainerId) : null,
    }),
  });
  saveBtn.textContent = 'Создать'; saveBtn.disabled = false;
  if (ok) {
    userModal.classList.remove('open');
    alert('Пользователь создан успешно!');
  } else {
    userModalError.textContent = getField(data, 'message') ?? 'Ошибка создания';
    userModalError.style.display = 'block';
  }
});

// Modal: добавить группу
const addGroupModal      = document.getElementById('add-group-modal');
const addGroupModalError = document.getElementById('add-group-modal-error');

if (addGroupModal) {
  document.getElementById('add-group-btn')?.addEventListener('click', async () => {
    addGroupModalError.style.display = 'none';
    // Загружаем направления и тренеров для селектов
    const dirs = await fetchCached('adminDirections', '/api/Directions');
    const trns = await fetchCached('adminTrainers', '/api/Trainers');
    const dirSelect = document.getElementById('ag-direction');
    const trnSelect = document.getElementById('ag-trainer');
    if (dirs && dirSelect) {
      dirSelect.innerHTML = dirs.map(d => `<option value="${d.direction_id ?? d.directionId}">${d.name}</option>`).join('');
    }
    if (trns && trnSelect) {
      trnSelect.innerHTML = trns.map(t => `<option value="${t.trainer_id ?? t.trainerId}">${t.lastName} ${t.firstName}</option>`).join('');
    }
    addGroupModal.classList.add('open');
  });
  document.getElementById('add-group-modal-cancel')?.addEventListener('click', () => addGroupModal.classList.remove('open'));
  addGroupModal.addEventListener('click', e => { if (e.target === addGroupModal) addGroupModal.classList.remove('open'); });

  document.getElementById('add-group-modal-save')?.addEventListener('click', async () => {
    const name        = document.getElementById('ag-name').value.trim();
    const directionId = document.getElementById('ag-direction').value;
    const trainerId   = document.getElementById('ag-trainer').value;
    const status      = document.getElementById('ag-status').value;
    addGroupModalError.style.display = 'none';
    if (!name) {
      addGroupModalError.textContent = 'Введите название группы';
      addGroupModalError.style.display = 'block'; return;
    }
    const saveBtn = document.getElementById('add-group-modal-save');
    saveBtn.textContent = 'Сохранение…'; saveBtn.disabled = true;
    const { ok, data } = await apiFetch('/api/Groups', {
      method: 'POST',
      body: JSON.stringify({ name, direction_id: parseInt(directionId), trainer_id: parseInt(trainerId), status }),
    });
    saveBtn.textContent = 'Сохранить'; saveBtn.disabled = false;
    if (ok) {
      addGroupModal.classList.remove('open');
      delete cache.adminGroups;
      const pane = document.getElementById('admin-groups');
      delete pane.dataset.loaded;
      loadPane('admin-groups');
    } else {
      addGroupModalError.textContent = getField(data, 'message') ?? 'Ошибка сохранения';
      addGroupModalError.style.display = 'block';
    }
  });
}

// Modal: изменить статус занятия
const statusModal = document.getElementById('status-modal');
document.getElementById('sm-cancel').addEventListener('click', () => statusModal.classList.remove('open'));
statusModal.addEventListener('click', e => { if (e.target === statusModal) statusModal.classList.remove('open'); });

document.getElementById('sm-save').addEventListener('click', async () => {
  const classId = document.getElementById('sm-class-id').value;
  const status  = document.getElementById('sm-status').value;
  const saveBtn = document.getElementById('sm-save');
  saveBtn.textContent = 'Сохранение…'; saveBtn.disabled = true;
  const { ok } = await apiFetch(`/api/classes/${classId}/status`, {
    method: 'PATCH',
    body: JSON.stringify({ status }),
  });
  saveBtn.textContent = 'Сохранить'; saveBtn.disabled = false;
  if (ok) {
    statusModal.classList.remove('open');
    delete cache.trainerSchedule;
    delete cache.adminSchedule;
    const pane = document.getElementById('trainer-schedule');
    delete pane.dataset.loaded;
    loadPane('trainer-schedule');
  } else {
    alert('Ошибка изменения статуса');
  }
});

// Modal: изменить статус группы
const groupStatusModal = document.getElementById('group-status-modal');
if (groupStatusModal) {
  document.getElementById('gs-cancel')?.addEventListener('click', () => groupStatusModal.classList.remove('open'));
  groupStatusModal.addEventListener('click', e => { if (e.target === groupStatusModal) groupStatusModal.classList.remove('open'); });

  document.getElementById('gs-save')?.addEventListener('click', async () => {
    const groupId = document.getElementById('gs-group-id').value;
    const status  = document.getElementById('gs-status').value;
    const saveBtn = document.getElementById('gs-save');
    saveBtn.textContent = 'Сохранение…'; saveBtn.disabled = true;
    const { ok } = await apiFetch(`/api/Groups/${groupId}/status`, {
      method: 'PATCH',
      body: JSON.stringify({ status }),
    });
    saveBtn.textContent = 'Сохранить'; saveBtn.disabled = false;
    if (ok) {
      groupStatusModal.classList.remove('open');
      delete cache.adminGroups;
      const pane = document.getElementById('admin-groups');
      delete pane.dataset.loaded;
      loadPane('admin-groups');
    } else {
      alert('Ошибка изменения статуса');
    }
  });
}

// Modal: изменить долг
const debtModal = document.getElementById('debt-modal');
document.getElementById('dm-cancel').addEventListener('click', () => debtModal.classList.remove('open'));
debtModal.addEventListener('click', e => { if (e.target === debtModal) debtModal.classList.remove('open'); });

document.getElementById('dm-save').addEventListener('click', async () => {
  const subId  = document.getElementById('dm-sub-id').value;
  const status = document.getElementById('dm-status').value;
  const saveBtn = document.getElementById('dm-save');
  saveBtn.textContent = 'Сохранение…'; saveBtn.disabled = true;
  const { ok } = await apiFetch(`/api/Subscriptions/${subId}/status`, {
    method: 'PATCH',
    body: JSON.stringify({ status }),
  });
  saveBtn.textContent = 'Сохранить'; saveBtn.disabled = false;
  if (ok) {
    debtModal.classList.remove('open');
    delete cache.accDebts;
    delete cache.accSummary;
    const pane = document.getElementById('acc-debts');
    delete pane.dataset.loaded;
    loadPane('acc-debts');
  } else {
    alert('Ошибка изменения статуса');
  }
});

// Modal: добавить долг (бухгалтер)
const addDebtModal      = document.getElementById('add-debt-modal');
const addDebtModalError = document.getElementById('add-debt-modal-error');

document.getElementById('add-debt-btn')?.addEventListener('click', async () => {
  addDebtModalError.style.display = 'none';
  const groups = await fetchCached('accGroups', '/api/Groups');
  const grpSelect = document.getElementById('ad-group');
  if (groups && grpSelect) {
    grpSelect.innerHTML = groups.map(g => `<option value="${g.group_id ?? g.groupId}">${g.name}</option>`).join('');
  }
  addDebtModal.classList.add('open');
});
addDebtModal?.addEventListener('click', e => { if (e.target === addDebtModal) addDebtModal.classList.remove('open'); });
document.getElementById('add-debt-modal-cancel')?.addEventListener('click', () => addDebtModal.classList.remove('open'));

document.getElementById('add-debt-modal-save')?.addEventListener('click', async () => {
  const clientId = document.getElementById('ad-clientId').value;
  const groupId  = document.getElementById('ad-group').value;
  const amount   = document.getElementById('ad-amount').value;
  addDebtModalError.style.display = 'none';
  if (!clientId || !amount) {
    addDebtModalError.textContent = 'Заполните все поля';
    addDebtModalError.style.display = 'block'; return;
  }
  const saveBtn = document.getElementById('add-debt-modal-save');
  saveBtn.textContent = 'Добавление…'; saveBtn.disabled = true;
  const { ok, data } = await apiFetch('/api/Finance/add-debt', {
    method: 'POST',
    body: JSON.stringify({ clientId: parseInt(clientId), groupId: parseInt(groupId), amount: parseFloat(amount) }),
  });
  saveBtn.textContent = 'Добавить'; saveBtn.disabled = false;
  if (ok) {
    addDebtModal.classList.remove('open');
    delete cache.accDebts;
    delete cache.accSummary;
    const pane = document.getElementById('acc-debts');
    delete pane.dataset.loaded;
    loadPane('acc-debts');
    loadPane('acc-summary');
  } else {
    addDebtModalError.textContent = getField(data, 'message') ?? 'Ошибка';
    addDebtModalError.style.display = 'block';
  }
});

// Modal: добавить тренера
const addTrainerModal      = document.getElementById('add-trainer-modal');
const addTrainerModalError = document.getElementById('add-trainer-modal-error');

document.getElementById('add-trainer-btn')?.addEventListener('click', async () => {
  addTrainerModalError.style.display = 'none';
  const dirs = await fetchCached('adminDirections', '/api/Directions');
  const dirSelect = document.getElementById('tr-direction');
  if (dirs && dirSelect) {
    dirSelect.innerHTML = dirs.map(d => `<option value="${d.direction_id ?? d.directionId}">${d.name}</option>`).join('');
  }
  ['tr-lastName','tr-firstName','tr-phone','tr-email'].forEach(id => document.getElementById(id).value = '');
  addTrainerModal.classList.add('open');
});
addTrainerModal?.addEventListener('click', e => { if (e.target === addTrainerModal) addTrainerModal.classList.remove('open'); });
document.getElementById('add-trainer-modal-cancel')?.addEventListener('click', () => addTrainerModal.classList.remove('open'));

document.getElementById('add-trainer-modal-save')?.addEventListener('click', async () => {
  const lastName    = document.getElementById('tr-lastName').value.trim();
  const firstName   = document.getElementById('tr-firstName').value.trim();
  const directionId = document.getElementById('tr-direction').value;
  const phone       = document.getElementById('tr-phone').value.trim();
  const email       = document.getElementById('tr-email').value.trim();
  addTrainerModalError.style.display = 'none';
  if (!lastName || !firstName) {
    addTrainerModalError.textContent = 'Заполните фамилию и имя';
    addTrainerModalError.style.display = 'block'; return;
  }
  const saveBtn = document.getElementById('add-trainer-modal-save');
  saveBtn.textContent = 'Сохранение…'; saveBtn.disabled = true;
  const { ok, data } = await apiFetch('/api/Trainers', {
    method: 'POST',
    body: JSON.stringify({ lastName, firstName, direction_id: parseInt(directionId), phone, email }),
  });
  saveBtn.textContent = 'Сохранить'; saveBtn.disabled = false;
 if (ok) {
  addTrainerModal.classList.remove('open');
  delete cache.adminTrainers;
  const pane = document.getElementById('admin-trainers');
  delete pane.dataset.loaded;
  loadPane('admin-trainers');
  }
});

// Modal: добавить занятие
const addClassModal      = document.getElementById('add-class-modal');
const addClassModalError = document.getElementById('add-class-modal-error');

document.getElementById('add-class-btn')?.addEventListener('click', async () => {
  addClassModalError.style.display = 'none';
  const groups  = await fetchCached('adminGroups',  '/api/Groups');
  const trainers = await fetchCached('adminTrainers', '/api/Trainers');
  const grpSelect = document.getElementById('ac-group');
  const trnSelect = document.getElementById('ac-trainer');
  if (groups && grpSelect)
    grpSelect.innerHTML = groups.map(g => `<option value="${g.group_id ?? g.groupId}">${g.name}</option>`).join('');
  if (trainers && trnSelect)
    trnSelect.innerHTML = trainers.map(t => `<option value="${t.trainer_id ?? t.trainerId}">${t.lastName} ${t.firstName}</option>`).join('');
  // Ставим сегодняшнюю дату по умолчанию
  document.getElementById('ac-date').value = new Date().toISOString().split('T')[0];
  document.getElementById('ac-time').value = '10:00';
  addClassModal.classList.add('open');
});
addClassModal?.addEventListener('click', e => { if (e.target === addClassModal) addClassModal.classList.remove('open'); });
document.getElementById('add-class-modal-cancel')?.addEventListener('click', () => addClassModal.classList.remove('open'));

document.getElementById('add-class-modal-save')?.addEventListener('click', async () => {
  const groupId   = document.getElementById('ac-group').value;
  const trainerId = document.getElementById('ac-trainer').value;
  const date      = document.getElementById('ac-date').value;
  const time      = document.getElementById('ac-time').value;
  const status    = document.getElementById('ac-status').value;
  addClassModalError.style.display = 'none';
  if (!groupId || !date || !time) {
    addClassModalError.textContent = 'Заполните все обязательные поля';
    addClassModalError.style.display = 'block'; return;
  }
  const saveBtn = document.getElementById('add-class-modal-save');
  saveBtn.textContent = 'Сохранение…'; saveBtn.disabled = true;
  const { ok, data } = await apiFetch('/api/Classes', {
    method: 'POST',
    body: JSON.stringify({
      group_id:   parseInt(groupId),
      trainer_id: parseInt(trainerId),
      date:       new Date(date).toISOString(),
      time:       time,
      status
    }),
  });
  saveBtn.textContent = 'Сохранить'; saveBtn.disabled = false;
  if (ok) {
    addClassModal.classList.remove('open');
    alert('Занятие добавлено!');
  } else {
    addClassModalError.textContent = getField(data, 'message') ?? 'Ошибка сохранения';
    addClassModalError.style.display = 'block';
  }
});

async function loadClassesForGroup(groupId) {
  const clsSelect = document.getElementById('reg-class');
  clsSelect.innerHTML = '<option value="">— Без конкретного занятия —</option>';
  if (!groupId) return;
  const { ok, data } = await apiFetch(`/api/Classes/bygroup/${groupId}`);
  if (ok && data?.length) {
    clsSelect.innerHTML += data
      .filter(c => c.status === 'Запланировано')
      .map(c => `<option value="${c.class_id}">${fmtDate(c.date)} ${c.time ?? ''}</option>`)
      .join('');
  }
}

// Modal: записать клиента в группу
const regClientModal      = document.getElementById('reg-client-modal');
const regClientModalError = document.getElementById('reg-client-modal-error');

document.getElementById('reg-client-modal-cancel')?.addEventListener('click', () => regClientModal.classList.remove('open'));
regClientModal?.addEventListener('click', e => { if (e.target === regClientModal) regClientModal.classList.remove('open'); });

document.getElementById('reg-client-modal-save')?.addEventListener('click', async () => {
  const clientId = document.getElementById('reg-client-id').value;
  const groupId  = document.getElementById('reg-group').value;
  const amount   = document.getElementById('reg-amount').value;
  regClientModalError.style.display = 'none';
  if (!clientId || !groupId) {
    regClientModalError.textContent = 'Выберите группу';
    regClientModalError.style.display = 'block'; return;
  }
  const saveBtn = document.getElementById('reg-client-modal-save');
  saveBtn.textContent = 'Запись…'; saveBtn.disabled = true;

  // Создаём регистрацию
  const { ok } = await apiFetch('/api/Registration', {
    method: 'POST',
    body: JSON.stringify({ client_id: parseInt(clientId), group_id: parseInt(groupId), registration_date: new Date().toISOString() }),
  });

  // Если указана сумма — создаём абонемент
  if (ok && amount) {
    await apiFetch('/api/Subscriptions', {
      method: 'POST',
      body: JSON.stringify({ client_id: parseInt(clientId), group_id: parseInt(groupId), amount: parseFloat(amount), status: 'Активен' }),
    });
  }

  saveBtn.textContent = 'Записать'; saveBtn.disabled = false;
  if (ok) {
    regClientModal.classList.remove('open');
    alert('Клиент записан в группу!');
  } else {
    regClientModalError.textContent = 'Ошибка записи';
    regClientModalError.style.display = 'block';
  }
});

// Initial load
if (ROLE === 'Trainer') {
  loadPane('trainer-schedule');
  loadPane('trainer-groups');
} else if (ROLE === 'Client') {
  loadPane('client-schedule');
  loadPane('client-groups');
  loadPane('client-payments');
} else if (ROLE === 'Director') {
  loadPane('dir-occupancy');
  loadPane('dir-popularity');
  loadPane('dir-workload');
} else if (ROLE === 'Accountant') {
  loadPane('acc-summary');
  loadPane('acc-revenue');
  loadPane('acc-debts');
} else if (ROLE === 'Admin') {
  loadPane('admin-clients');
  loadPane('admin-trainers');
  loadPane('admin-groups');
  loadPane('admin-schedule');
  document.getElementById('add-client-btn').style.display = '';
  document.getElementById('add-user-btn').style.display   = '';
}