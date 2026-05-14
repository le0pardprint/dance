import React, { useState, useEffect } from 'react';

const API_URL = 'https://localhost:7127';

// Утилита: читаем поле без учёта регистра (Token / token / TOKEN — всё найдётся)
function getField(obj, ...keys) {
  if (!obj) return undefined;
  for (const key of keys) {
    const found = Object.keys(obj).find(k => k.toLowerCase() === key.toLowerCase());
    if (found !== undefined) return obj[found];
  }
  return undefined;
}

const ROLE_LABELS = {
  Client: 'Клиент',
  Trainer: 'Тренер',
  Director: 'Директор',
  Accountant: 'Бухгалтер',
  Admin: 'Администратор',
};

const ROLE_URLS = {
  Client: '/api/Client/schedule',
  Trainer: '/api/Trainer/schedule',
  Director: '/api/Analytics/groups/occupancy',
  Accountant: '/api/Finance/revenue',
  Admin: '/api/Clients',
};

// ——— Рендер данных по ролям ———
function ScheduleTable({ items }) {
  if (!items?.length) return <p className="empty">Расписание пусто</p>;
  return (
    <table>
      <thead>
        <tr>
          <th>Дата</th><th>Время</th><th>Группа</th><th>Направление</th><th>Тренер</th><th>Статус</th>
        </tr>
      </thead>
      <tbody>
        {items.map((c, i) => (
          <tr key={i}>
            <td>{c.date ? new Date(c.date).toLocaleDateString('ru-RU') : '—'}</td>
            <td>{c.time ?? '—'}</td>
            <td>{c.groupName ?? '—'}</td>
            <td>{c.directionName ?? '—'}</td>
            <td>{c.trainerName ?? '—'}</td>
            <td><span className={`badge badge-${c.status}`}>{c.status ?? '—'}</span></td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}

function OccupancyView({ data }) {
  if (!data) return null;
  return (
    <>
      <div className="stats-row">
        <div className="stat-card"><div className="stat-num">{data.totalGroups}</div><div className="stat-label">Групп</div></div>
        <div className="stat-card"><div className="stat-num">{data.totalStudents}</div><div className="stat-label">Учеников</div></div>
        <div className="stat-card"><div className="stat-num">{data.averageOccupancyPercent}%</div><div className="stat-label">Заполненность</div></div>
      </div>
      <table>
        <thead><tr><th>Группа</th><th>Статус</th><th>Учеников</th><th>Макс.</th><th>%</th></tr></thead>
        <tbody>
          {data.groups?.map((g, i) => (
            <tr key={i}>
              <td>{g.name}</td>
              <td><span className={`badge badge-${g.status}`}>{g.status}</span></td>
              <td>{g.studentsCount}</td>
              <td>{g.maxCapacity}</td>
              <td>
                <div className="progress-bar">
                  <div className="progress-fill" style={{ width: `${Math.min(g.occupancyPercent, 100)}%` }} />
                  <span>{g.occupancyPercent}%</span>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
}

function RevenueView({ data }) {
  if (!data) return null;
  return (
    <div className="stats-row">
      <div className="stat-card accent">
        <div className="stat-num">{Number(data.totalRevenue ?? 0).toLocaleString('ru-RU')} ₽</div>
        <div className="stat-label">Общая выручка</div>
      </div>
    </div>
  );
}

function ClientsTable({ items }) {
  if (!Array.isArray(items)) return <pre>{JSON.stringify(items, null, 2)}</pre>;
  if (!items.length) return <p className="empty">Нет клиентов</p>;
  return (
    <table>
      <thead><tr><th>ID</th><th>Фамилия</th><th>Имя</th><th>Возраст</th><th>Телефон</th><th>Email</th></tr></thead>
      <tbody>
        {items.map((c, i) => (
          <tr key={i}>
            <td>{c.client_id ?? c.clientId ?? i + 1}</td>
            <td>{c.lastName ?? '—'}</td>
            <td>{c.firstName ?? '—'}</td>
            <td>{c.age ?? '—'}</td>
            <td>{c.phone ?? '—'}</td>
            <td>{c.email ?? '—'}</td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}

function DataView({ role, data }) {
  if (!data) return null;
  if (data.error) return <div className="error-box">⚠ {data.error}</div>;

  if (role === 'Client' || role === 'Trainer') {
    const items = Array.isArray(data) ? data : [];
    return (
      <section className="data-section">
        <h3>📅 Расписание</h3>
        <ScheduleTable items={items} />
      </section>
    );
  }
  if (role === 'Director') return (
    <section className="data-section"><h3>📊 Заполненность групп</h3><OccupancyView data={data} /></section>
  );
  if (role === 'Accountant') return (
    <section className="data-section"><h3>💰 Финансы</h3><RevenueView data={data} /></section>
  );
  if (role === 'Admin') return (
    <section className="data-section"><h3>👥 Клиенты</h3><ClientsTable items={data} /></section>
  );
  return <pre className="raw-json">{JSON.stringify(data, null, 2)}</pre>;
}

// ——— Главный компонент ———
export default function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [role, setRole] = useState(null);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loginError, setLoginError] = useState('');
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(false);
  const [dataError, setDataError] = useState('');

  useEffect(() => {
    const token = localStorage.getItem('token');
    const userRole = localStorage.getItem('role');
    if (token && userRole) {
      setIsAuthenticated(true);
      setRole(userRole);
    }
  }, []);

  const handleLogin = async (e) => {
    e.preventDefault();
    setLoginError('');
    try {
      const response = await fetch(`${API_URL}/api/Auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password }),
      });

      const json = await response.json();

      if (response.ok) {
        // Защита от регистра: Token/token, Role/role, UserId/userId
        const token  = getField(json, 'token');
        const role   = getField(json, 'role');
        const userId = getField(json, 'userId', 'user_id');

        if (!token) {
          setLoginError('Сервер не вернул токен. Проверьте AuthResponseDto.');
          console.error('Ответ сервера:', json);
          return;
        }

        localStorage.setItem('token', token);
        localStorage.setItem('role', role ?? '');
        localStorage.setItem('userId', userId ?? '');
        setIsAuthenticated(true);
        setRole(role ?? '');
      } else {
        setLoginError(getField(json, 'message') ?? 'Ошибка входа');
      }
    } catch (err) {
      console.error(err);
      setLoginError('Ошибка подключения к серверу. Убедитесь, что API запущен на ' + API_URL);
    }
  };

  const handleLogout = () => {
    localStorage.clear();
    setIsAuthenticated(false);
    setRole(null);
    setData(null);
    setDataError('');
  };

  const fetchDataByRole = async () => {
    const token = localStorage.getItem('token');
    const currentRole = localStorage.getItem('role');
    setDataError('');
    setData(null);

    if (!token) { setDataError('Нет токена. Войдите заново.'); return; }

    const url = ROLE_URLS[currentRole];
    if (!url) { setDataError('Неизвестная роль: ' + currentRole); return; }

    setLoading(true);
    try {
      const response = await fetch(`${API_URL}${url}`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (response.status === 401) {
        setDataError('Ошибка авторизации (401). Токен истёк или недействителен.');
        localStorage.clear();
        setIsAuthenticated(false);
        return;
      }
      if (!response.ok) {
        const text = await response.text();
        setDataError(`Ошибка ${response.status}: ${text}`);
        return;
      }

      const result = await response.json();
      setData(result);
    } catch (err) {
      console.error(err);
      setDataError('Ошибка подключения. Проверьте CORS и что API запущен.');
    } finally {
      setLoading(false);
    }
  };

  // ——— Страница входа ———
  if (!isAuthenticated) {
    return (
      <>
        <style>{styles}</style>
        <div className="login-wrap">
          <div className="login-card">
            <div className="login-logo">💃</div>
            <h1>CRM Школа танцев</h1>
            <p className="login-sub">Войдите в систему</p>
            {loginError && <div className="error-box">{loginError}</div>}
            <form onSubmit={handleLogin}>
              <label>Email</label>
              <input type="email" value={email} onChange={e => setEmail(e.target.value)}
                placeholder="admin@dance.ru" required />
              <label>Пароль</label>
              <input type="password" value={password} onChange={e => setPassword(e.target.value)}
                placeholder="••••••••" required />
              <button type="submit" className="btn-primary">Войти</button>
            </form>
          </div>
        </div>
      </>
    );
  }

  // ——— Основной интерфейс ———
  return (
    <>
      <style>{styles}</style>
      <div className="app">
        <header className="navbar">
          <div className="nav-brand">💃 Танцевальная школа CRM</div>
          <div className="nav-right">
            <span className="role-badge">{ROLE_LABELS[role] ?? role}</span>
            <button className="btn-outline" onClick={fetchDataByRole} disabled={loading}>
              {loading ? '⏳ Загрузка...' : '📋 Мои данные'}
            </button>
            <button className="btn-ghost" onClick={handleLogout}>Выйти</button>
          </div>
        </header>

        <main className="main">
          {dataError && <div className="error-box">{dataError}</div>}
          {!data && !dataError && !loading && (
            <div className="welcome-card">
              <div className="welcome-icon">👋</div>
              <h2>Добро пожаловать!</h2>
              <p>Нажмите <strong>«Мои данные»</strong>, чтобы загрузить информацию</p>
            </div>
          )}
          {data && <DataView role={role} data={data} />}
        </main>
      </div>
    </>
  );
}

// ——— Стили ———
const styles = `
  *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

  body { font-family: 'Segoe UI', system-ui, sans-serif; background: #f0f2f5; color: #1a1a2e; }

  /* Login */
  .login-wrap { min-height: 100vh; display: flex; align-items: center; justify-content: center;
    background: linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%); }
  .login-card { background: #fff; border-radius: 16px; padding: 40px; width: 100%; max-width: 400px;
    box-shadow: 0 20px 60px rgba(0,0,0,0.3); }
  .login-logo { font-size: 48px; text-align: center; margin-bottom: 8px; }
  .login-card h1 { text-align: center; font-size: 22px; font-weight: 700; color: #1a1a2e; margin-bottom: 4px; }
  .login-sub { text-align: center; color: #888; margin-bottom: 24px; font-size: 14px; }
  .login-card label { display: block; font-size: 13px; font-weight: 600; color: #555; margin-bottom: 5px; margin-top: 14px; }
  .login-card input { width: 100%; padding: 10px 14px; border: 1.5px solid #ddd; border-radius: 8px;
    font-size: 15px; outline: none; transition: border-color .2s; }
  .login-card input:focus { border-color: #e94560; }

  /* Buttons */
  .btn-primary { width: 100%; margin-top: 20px; padding: 12px; background: #e94560;
    color: #fff; border: none; border-radius: 8px; font-size: 15px; font-weight: 600;
    cursor: pointer; transition: background .2s; }
  .btn-primary:hover { background: #c73652; }
  .btn-outline { padding: 8px 16px; border: 1.5px solid #e94560; color: #e94560; background: transparent;
    border-radius: 8px; font-size: 14px; cursor: pointer; transition: all .2s; }
  .btn-outline:hover:not(:disabled) { background: #e94560; color: #fff; }
  .btn-outline:disabled { opacity: 0.5; cursor: not-allowed; }
  .btn-ghost { padding: 8px 14px; border: none; background: rgba(255,255,255,0.15);
    color: #fff; border-radius: 8px; font-size: 14px; cursor: pointer; }
  .btn-ghost:hover { background: rgba(255,255,255,0.25); }

  /* Navbar */
  .navbar { display: flex; align-items: center; justify-content: space-between;
    padding: 0 24px; height: 60px; background: linear-gradient(90deg, #1a1a2e, #0f3460);
    color: #fff; box-shadow: 0 2px 8px rgba(0,0,0,0.2); position: sticky; top: 0; z-index: 10; }
  .nav-brand { font-size: 18px; font-weight: 700; }
  .nav-right { display: flex; align-items: center; gap: 12px; }
  .role-badge { background: rgba(233,69,96,0.3); border: 1px solid #e94560;
    color: #ff8099; padding: 4px 12px; border-radius: 20px; font-size: 13px; font-weight: 600; }

  /* Main */
  .main { padding: 24px; max-width: 1100px; margin: 0 auto; }

  /* Welcome */
  .welcome-card { background: #fff; border-radius: 16px; padding: 60px 40px;
    text-align: center; box-shadow: 0 2px 16px rgba(0,0,0,0.07); }
  .welcome-icon { font-size: 56px; margin-bottom: 16px; }
  .welcome-card h2 { font-size: 24px; margin-bottom: 10px; }
  .welcome-card p { color: #666; font-size: 15px; }

  /* Data section */
  .data-section { background: #fff; border-radius: 16px; padding: 24px;
    box-shadow: 0 2px 16px rgba(0,0,0,0.07); }
  .data-section h3 { font-size: 18px; font-weight: 700; margin-bottom: 20px; color: #1a1a2e; }

  /* Stats */
  .stats-row { display: flex; gap: 16px; margin-bottom: 24px; flex-wrap: wrap; }
  .stat-card { flex: 1; min-width: 120px; background: #f8f9ff; border-radius: 12px;
    padding: 20px; text-align: center; border: 1px solid #e8ebff; }
  .stat-card.accent { background: linear-gradient(135deg, #e94560, #c73652); color: #fff; border: none; }
  .stat-card.accent .stat-label { color: rgba(255,255,255,0.8); }
  .stat-num { font-size: 28px; font-weight: 800; color: #1a1a2e; }
  .stat-card.accent .stat-num { color: #fff; }
  .stat-label { font-size: 12px; color: #888; margin-top: 4px; text-transform: uppercase; letter-spacing: .5px; }

  /* Table */
  table { width: 100%; border-collapse: collapse; font-size: 14px; }
  th { background: #f0f2f5; padding: 10px 12px; text-align: left;
    font-weight: 600; color: #555; font-size: 12px; text-transform: uppercase; letter-spacing: .4px; }
  td { padding: 10px 12px; border-bottom: 1px solid #f0f2f5; color: #333; }
  tr:hover td { background: #fafbff; }

  /* Badge */
  .badge { display: inline-block; padding: 3px 10px; border-radius: 20px; font-size: 12px; font-weight: 600; }
  .badge-Активна, .badge-Проведено { background: #d1fae5; color: #065f46; }
  .badge-Запланировано { background: #dbeafe; color: #1e40af; }
  .badge-Завершена, .badge-Истёк { background: #fee2e2; color: #991b1b; }

  /* Progress bar */
  .progress-bar { display: flex; align-items: center; gap: 8px; }
  .progress-fill { height: 6px; background: #e94560; border-radius: 3px; flex-shrink: 0; }
  .progress-bar span { font-size: 12px; color: #888; white-space: nowrap; }

  /* Error / raw */
  .error-box { background: #fee2e2; border: 1px solid #fca5a5; color: #991b1b;
    padding: 12px 16px; border-radius: 10px; margin-bottom: 16px; font-size: 14px; }
  .raw-json { background: #1a1a2e; color: #a8ff78; padding: 20px; border-radius: 12px;
    font-size: 13px; overflow-x: auto; }
  .empty { color: #aaa; padding: 20px 0; text-align: center; }
`;