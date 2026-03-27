// layout.js — renders sidebar + topbar
// Call renderLayout('PageName') on each page

function renderLayout(activePage) {
    const pages = ['Dashboard', 'Student', 'Teachers', 'Courses', 'Enrollment', 'Attendance'];
    const hrefs = {
        Dashboard: '/LogIn/Dashboard',
        Student: '/LogIn/Student',
        Teachers: '/LogIn/Teacher',
        Courses: '/LogIn/Course',
        Enrollment: '/LogIn/Enrollment',
        Attendance: '/LogIn/Attendance'
    };

    const sidebar = document.getElementById('sidebar');
    if (sidebar) {
        sidebar.innerHTML = `
      <img src="https://upload.wikimedia.org/wikipedia/commons/thumb/8/8e/Don_Bosco_Technical_College_%28Cebu%29_seal.png/240px-Don_Bosco_Technical_College_%28Cebu%29_seal.png"
        class="sidebar-logo" alt="Logo" />
      <nav>
        ${pages.map(p => `<a href="${hrefs[p]}" class="${p === activePage ? 'active' : ''}">${p}</a>`).join('')}
      </nav>
    `;
    }

    const topbar = document.getElementById('topbar');
    if (topbar) {
        topbar.innerHTML = `
      <div class="search-box">
        <span>🔍</span>
        <input type="text" placeholder="Search here" />
      </div>
      <div class="topbar-right">
        <span style="font-size:1.3rem;color:#fff;cursor:pointer;">⚙️</span>
        <span style="font-size:1.3rem;color:#fff;cursor:pointer;">🔔</span>
        <span class="user-name">Junel Kevin</span>
        <span class="avatar"></span>
      </div>
    `;
    }
}
