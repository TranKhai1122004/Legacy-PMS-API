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

  const handlePmsAction = async (e) => {
    e.preventDefault();

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
  };

  const handleFetchBill = async () => {
    if (!searchRoom) {
      alert("Nhập số phòng");
      return;
    }

    const res = await fetch(`${API_BASE_URL}/api/bill?room=${searchRoom}`);

    if (!res.ok) {
      alert("Không tìm thấy dữ liệu");
      setRoomData(null);
      return;
    }

    const data = await res.json();
    setRoomData(data);
  };

  if (!isLoggedIn) {
    return (
      <div className="login-page">
        <div className="login-card">
          <h2>🏨 Hệ Thống PMS</h2>

          <form onSubmit={handleLogin}>
            <input
              className="input"
              type="text"
              placeholder="Tên đăng nhập"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
            />

            <input
              className="input"
              type="password"
              placeholder="Mật khẩu"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />

            <button className="btn btn-primary">
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
    <div className="dashboard">

      <div className="header">
        <h2>Dashboard Quản Lý Khách Sạn</h2>

        <button
          className="btn btn-danger"
          onClick={handleLogout}
        >
          Đăng xuất
        </button>
      </div>

      <div className="grid-container">

        <div className="card">

          <h3 className="card-title">
            Tính năng PMS
          </h3>

          <form onSubmit={handlePmsAction}>

            <div className="form-group">
              <label>Hành động</label>

              <select
                className="input"
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
                className="input"
                value={room}
                onChange={(e) => setRoom(e.target.value)}
              />
            </div>

            {action === "checkin" && (
              <>
                <input
                  className="input"
                  placeholder="Họ"
                  value={firstName}
                  onChange={(e) => setFirstName(e.target.value)}
                />

                <input
                  className="input"
                  placeholder="Tên"
                  value={lastName}
                  onChange={(e) => setLastName(e.target.value)}
                />
              </>
            )}

            {action === "roommove" && (
              <input
                className="input"
                placeholder="Phòng mới"
                value={newRoom}
                onChange={(e) => setNewRoom(e.target.value)}
              />
            )}

            {action === "wakeup" && (
              <div className="form-group">
                <label>Thời gian báo thức</label>

                <input
                  className="input"
                  type="time"
                  value={wakeUpTime}
                  onChange={(e) => setWakeUpTime(e.target.value)}
                  onClick={(e) => e.target.showPicker?.()}
                  onFocus={(e) => e.target.showPicker?.()}
                />
              </div>
            )}

            <button className="btn btn-success">
              Thực hiện
            </button>

          </form>

        </div>

        <div className="card">

          <h3 className="card-title">
            Tra cứu hóa đơn
          </h3>

          <input
            className="input"
            placeholder="Số phòng"
            value={searchRoom}
            onChange={(e) => setSearchRoom(e.target.value)}
          />

          <button
            className="btn btn-info"
            onClick={handleFetchBill}
          >
            Tra cứu
          </button>

          {roomData && (
            <>

              <div className="bill-amount">
                Tổng tiền: {roomData.totalCost?.toLocaleString()} VNĐ
              </div>

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
                      <td>{call.status}</td>
                    </tr>

                  ))}

                </tbody>

              </table>

            </>
          )}

        </div>

      </div>

    </div>
  );
}

export default App;