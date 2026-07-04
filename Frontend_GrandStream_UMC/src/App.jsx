import React, { useState } from 'react';
import './App.css';

const API_BASE_URL = "http://localhost:5274";

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(localStorage.getItem('isLoggedIn') === 'true');
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [loginError, setLoginError] = useState('');

  const [action, setAction] = useState('checkin');
  const [room, setRoom] = useState('');
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [newRoom, setNewRoom] = useState('');
  const [wakeUpTime, setWakeUpTime] = useState('');

  const [searchRoom, setSearchRoom] = useState('');
  const [roomData, setRoomData] = useState(null);

  const [errors, setErrors] = useState({});
  const [billingError, setBillingError] = useState('');

  const handleLogin = async (e) => {
    e.preventDefault();

    try {
      const res = await fetch(`${API_BASE_URL}/api/login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({ username, password })
      });

      const data = await res.json();

      if (data.success) {
        localStorage.setItem("isLoggedIn", "true");
        setIsLoggedIn(true);
        setLoginError("");
      } else {
        setLoginError(data.message);
      }
    } catch {
      setLoginError("Không thể kết nối Backend .NET");
    }
  };

  const handleLogout = () => {
    localStorage.removeItem("isLoggedIn");
    setIsLoggedIn(false);
    setRoomData(null);
  };

  const validatePmsForm = () => {
    const newErrors = {};

    if (!room.trim()) {
      newErrors.room = 'Số phòng không được để trống';
    }

    if (action === 'checkin') {
      if (!firstName.trim()) {
        newErrors.firstName = 'Họ không được để trống';
      }

      if (!lastName.trim()) {
        newErrors.lastName = 'Tên không được để trống';
      }
    }

    if (action === 'roommove') {
      if (!newRoom.trim()) {
        newErrors.newRoom = 'Phòng mới không được để trống';
      }
    }

    if (action === 'wakeup') {
      if (!wakeUpTime.trim()) {
        newErrors.wakeUpTime = 'Thời gian báo thức không được để trống';
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };
  const handlePmsAction = async (e) => {
    e.preventDefault();

    if (!validatePmsForm()) return;

    try {
      const res = await fetch(`${API_BASE_URL}/api/action`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          action,
          room,
          firstName,
          lastName,
          newRoom,
          wakeUpTime
        })
      });

      const data = await res.json();
      alert(data.message);
    } catch {
      alert("Không thể kết nối Backend .NET");
    }
  };

  const handleFetchBill = async () => {
    if (!searchRoom.trim()) {
      setBillingError("Số phòng không được để trống");
      setRoomData(null);
      return;
    }

    setBillingError("");

    try {
      const res = await fetch(`${API_BASE_URL}/api/bill?room=${searchRoom}`);

      if (!res.ok) {
        alert("Không tìm thấy dữ liệu");
        setRoomData(null);
        return;
      }

      const data = await res.json();
      setRoomData(data);
    } catch {
      alert("Không thể kết nối Backend .NET");
      setRoomData(null);
    }
  };

  if (!isLoggedIn) {
    return (
      <div className="login-page">
        <div className="login-card">
          <div className="login-badge">Hotel PMS</div>
          <h2>Hệ Thống Quản Lý Khách Sạn</h2>
          <p className="login-subtitle">
            Đăng nhập để quản lý check-in, room move, wake-up call và tra cứu hóa đơn.
          </p>

          <form onSubmit={handleLogin}>
            <div className="form-group">
              <label>Tên đăng nhập</label>
              <input
                className="input"
                type="text"
                placeholder="Nhập tên đăng nhập"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
              />
            </div>

            <div className="form-group">
              <label>Mật khẩu</label>
              <input
                className="input"
                type="password"
                placeholder="Nhập mật khẩu"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </div>

            <button className="btn btn-primary btn-full">
              Đăng nhập
            </button>

            {loginError && (
              <p className="error">
                {loginError}
              </p>
            )}
          </form>
        </div>
      </div>
    );
  }

  return (
    <div className="dashboard-page">
      <aside className="sidebar">
        <div className="sidebar-brand">
          <div className="brand-logo">🏨</div>
          <div>
            <h3>Hotel PMS</h3>
            <p>Management System</p>
          </div>
        </div>

        <div className="sidebar-menu">
          <div className="menu-item active">Dashboard</div>
          <div className="menu-item">PMS Actions</div>
          <div className="menu-item">Billing</div>
          <div className="menu-item">Rooms</div>
        </div>
      </aside>

      <main className="dashboard-main">
        <div className="header">
          <div>
            <p className="page-label">Hotel Management</p>
            <h2>Dashboard Quản Lý Khách Sạn</h2>
          </div>

          <button
            className="btn btn-danger"
            onClick={handleLogout}
          >
            Đăng xuất
          </button>
        </div>

        {/* <div className="stats-grid">
          <div className="stat-card">
            <span className="stat-label">Module</span>
            <h3>PMS Action</h3>
            <p>Check-in, Check-out, Room Move, Wake-up Call</p>
          </div>

          <div className="stat-card">
            <span className="stat-label">Billing</span>
            <h3>Tra cứu hóa đơn</h3>
            <p>Xem tổng tiền và lịch sử cuộc gọi theo phòng</p>
          </div>
        </div> */}

        <div className="grid-container">
          <div className="card">
            <div className="card-head">
              <div>
                <p className="card-kicker">Operations</p>
                <h3 className="card-title">Tính năng PMS</h3>
              </div>
            </div>

            <form onSubmit={handlePmsAction}>
              <div className="form-group">
                <label>Hành động</label>
                <select
                  className="input select-input"
                  value={action}
                  onChange={(e) => setAction(e.target.value)}
                >
                  <option value="checkin">Check-In</option>
                  <option value="checkout">Check-Out</option>
                  <option value="roommove">Room Move</option>
                  <option value="wakeup">Wakeup Call</option>
                </select>
              </div>

              <div className="form-group">
                <label>Số phòng</label>
                <input
                  className={`input ${errors.room ? 'input-error' : ''}`}
                  placeholder="Ví dụ: 305"
                  value={room}
                  onChange={(e) => {
                    setRoom(e.target.value);
                    setErrors((prev) => ({ ...prev, room: '' }));
                  }}
                />
                {errors.room && <p className="field-error">{errors.room}</p>}
              </div>

              {action === "checkin" && (
                <div className="form-row">
                  <div className="field-wrap">
                    <input
                      className={`input ${errors.firstName ? 'input-error' : ''}`}
                      placeholder="Họ"
                      value={firstName}
                      onChange={(e) => {
                        setFirstName(e.target.value);
                        setErrors((prev) => ({ ...prev, firstName: '' }));
                      }}
                    />
                    {errors.firstName && <p className="field-error">{errors.firstName}</p>}
                  </div>

                  <div className="field-wrap">
                    <input
                      className={`input ${errors.lastName ? 'input-error' : ''}`}
                      placeholder="Tên"
                      value={lastName}
                      onChange={(e) => {
                        setLastName(e.target.value);
                        setErrors((prev) => ({ ...prev, lastName: '' }));
                      }}
                    />
                    {errors.lastName && <p className="field-error">{errors.lastName}</p>}
                  </div>
                </div>
              )}

              {action === "roommove" && (
                <div className="form-group">
                  <input
                    className={`input ${errors.newRoom ? 'input-error' : ''}`}
                    placeholder="Phòng mới"
                    value={newRoom}
                    onChange={(e) => {
                      setNewRoom(e.target.value);
                      setErrors((prev) => ({ ...prev, newRoom: '' }));
                    }}
                  />
                  {errors.newRoom && <p className="field-error">{errors.newRoom}</p>}
                </div>
              )}

              {action === "wakeup" && (
                <div className="form-group">
                  <label>Thời gian báo thức</label>
                  <input
                    className={`input ${errors.wakeUpTime ? 'input-error' : ''}`}
                    type="time"
                    value={wakeUpTime}
                    onChange={(e) => {
                      setWakeUpTime(e.target.value);
                      setErrors((prev) => ({ ...prev, wakeUpTime: '' }));
                    }}
                    onClick={(e) => e.target.showPicker?.()}
                    onFocus={(e) => e.target.showPicker?.()}
                  />
                  {errors.wakeUpTime && <p className="field-error">{errors.wakeUpTime}</p>}
                </div>
              )}

              <button className="btn btn-success">
                Thực hiện
              </button>
            </form>
          </div>

          <div className="card">
            <div className="card-head">
              <div>
                <p className="card-kicker">Billing</p>
                <h3 className="card-title">Tra cứu hóa đơn</h3>
              </div>
            </div>

            <div className="search-box">
              <div className="billing-search-wrap">
                <input
                  className={`input ${billingError ? 'input-error' : ''}`}
                  placeholder="Nhập số phòng cần tra cứu"
                  value={searchRoom}
                  onChange={(e) => {
                    setSearchRoom(e.target.value);
                    setBillingError('');
                  }}
                />
                {billingError && <p className="field-error">{billingError}</p>}
              </div>

              <button
                className="btn btn-info"
                onClick={handleFetchBill}
              >
                Tra cứu
              </button>
            </div>

            {roomData && (
              <>
                <div className="bill-summary">
                  <div className="bill-label">Tổng chi phí</div>
                  <div className="bill-amount">
                    {roomData.totalCost?.toLocaleString()} VNĐ
                  </div>
                </div>

                <div className="table-wrapper">
                  <table className="table">
                    <thead>
                      <tr>
                        <th>Callee</th>
                        <th>Duration</th>
                        <th>Status</th>
                      </tr>
                    </thead>

                    <tbody>
                      {roomData.calls?.map((call, index) => (
                        <tr key={index}>
                          <td>{call.callee}</td>
                          <td>{call.duration}</td>
                          <td>
                            <span className="status-badge">
                              {call.status}
                            </span>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </>
            )}
          </div>
        </div>
      </main>
    </div>
  );
}

export default App;