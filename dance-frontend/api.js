// ─── Конфиг ───────────────────────────────────────────────────────────────────
const API_URL = 'http://localhost:5052';

// ─── Auth helpers ─────────────────────────────────────────────────────────────
function getToken()  { return localStorage.getItem('token'); }
function getRole()   { return localStorage.getItem('role'); }
function getEmail()  { return localStorage.getItem('email'); }
function getUserId() { return localStorage.getItem('userId'); }

function saveAuth(token, role, email, userId) {
  localStorage.setItem('token',  token  ?? '');
  localStorage.setItem('role',   role   ?? '');
  localStorage.setItem('email',  email  ?? '');
  localStorage.setItem('userId', userId ?? '');
}

function clearAuth() { localStorage.clear(); }

// ─── Case-insensitive field reader ────────────────────────────────────────────
function getField(obj, ...keys) {
  if (!obj || typeof obj !== 'object') return undefined;
  for (const key of keys) {
    const found = Object.keys(obj).find(k => k.toLowerCase() === key.toLowerCase());
    if (found !== undefined) return obj[found];
  }
}

// ─── Fetch wrapper ────────────────────────────────────────────────────────────
async function apiFetch(path, options = {}) {
  const token = getToken();
  try {
    const res = await fetch(`${API_URL}${path}`, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
        ...options.headers,
      },
    });
    const text = await res.text();
    const data = text ? JSON.parse(text) : null;
    return { ok: res.ok, status: res.status, data };
  } catch {
    return { ok: false, status: 0, data: null };
  }
}

// ─── Formatters ───────────────────────────────────────────────────────────────
function fmtDate(d) {
  if (!d) return '—';
  return new Date(d).toLocaleDateString('ru-RU', { day:'2-digit', month:'2-digit', year:'numeric' });
}

function fmtMoney(n) {
  if (n == null) return '—';
  return Number(n).toLocaleString('ru-RU') + ' ₽';
}

// ─── Badge ────────────────────────────────────────────────────────────────────
function badge(status) {
  const map = {
    'Активен':'success','Активна':'success','Оплачен':'success','Проведено':'success',
    'Запланировано':'info','Ожидает':'warn',
    'Завершена':'muted','Истёк':'danger','Задолженность':'danger',
  };
  const cls = map[status] || 'muted';
  return `<span class="badge badge-${cls}">${status ?? '—'}</span>`;
}

// ─── Progress bar ─────────────────────────────────────────────────────────────
function progressBar(value) {
  const pct   = Math.min(value ?? 0, 100);
  const color = pct > 80 ? 'var(--success)' : pct > 50 ? 'var(--accent)' : 'var(--warn)';
  return `<div class="progress-wrap">
    <div class="progress-bg"><div class="progress-fill" style="width:${pct}%;background:${color}"></div></div>
    <span class="progress-label">${pct}%</span>
  </div>`;
}

// ─── Table builder ────────────────────────────────────────────────────────────
function buildTable(cols, rows, emptyText = 'Нет данных') {
  if (!rows || !rows.length) {
    return `<div class="empty-state"><div class="empty-icon">🌿</div>${emptyText}</div>`;
  }
  const ths = cols.map(c => `<th>${c}</th>`).join('');
  const trs = rows.map(row =>
    `<tr>${row.map(cell => `<td>${cell ?? '—'}</td>`).join('')}</tr>`
  ).join('');
  return `<div class="table-wrap"><table>
    <thead><tr>${ths}</tr></thead>
    <tbody>${trs}</tbody>
  </table></div>`;
}

// ─── Stats builder ────────────────────────────────────────────────────────────
function buildStats(items) {
  return `<div class="stats-grid">${items.map(([val, label, accent]) =>
    `<div class="stat-card${accent ? ' accent' : ''}">
      <div class="stat-val">${val ?? '—'}</div>
      <div class="stat-label">${label}</div>
    </div>`
  ).join('')}</div>`;
}

// ─── Loading / Error ──────────────────────────────────────────────────────────
function showLoading(el) {
  el.innerHTML = `<div class="loading-state"><div class="spinner"></div>Загрузка...</div>`;
}

function showErr(el, text) {
  el.innerHTML = `<div class="error-box">${text}</div>`;
}
