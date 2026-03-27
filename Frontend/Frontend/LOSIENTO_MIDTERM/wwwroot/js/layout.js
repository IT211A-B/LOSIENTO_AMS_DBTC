// Global data store shared across pages
window.AMS = window.AMS || {
    students: {
        BSIT: [
            { name: 'Kenwell Montekun', id: '902182', email: 'Kenwell.Montekun@dbtc-cebu.edu.ph', course: 'BSIT', year: 1 },
            { name: 'Kenwell Montekun', id: '902182', email: 'Kenwell.Montekun@dbtc-cebu.edu.ph', course: 'BSIT', year: 2 },
            { name: 'Kenwell Montekun', id: '902182', email: 'Kenwell.Montekun@dbtc-cebu.edu.ph', course: 'BSIT', year: 3 },
            { name: 'Kenwell Montekun', id: '902182', email: 'Kenwell.Montekun@dbtc-cebu.edu.ph', course: 'BSIT', year: 4 },
            { name: 'Kenwell Montekun', id: '902182', email: 'Kenwell.Montekun@dbtc-cebu.edu.ph', course: 'BSIT', year: 1 },
            { name: 'Kenwell Montekun', id: '902182', email: 'Kenwell.Montekun@dbtc-cebu.edu.ph', course: 'BSIT', year: 3 },
        ],
        BSME: [
            { name: 'Kenwell Montekun', id: '902182', email: 'Kenwell.Montekun@dbtc-cebu.edu.ph', course: 'BSME', year: 2 },
            { name: 'Kenwell Montekun', id: '902182', email: 'Kenwell.Montekun@dbtc-cebu.edu.ph', course: 'BSME', year: 4 },
            { name: 'Kenwell Montekun', id: '902182', email: 'Kenwell.Montekun@dbtc-cebu.edu.ph', course: 'BSME', year: 1 },
        ]
    },
    teachers: [
        { name: 'Moreno Juqjun', id: '902182', subject: 'Theology', position: 'Instructor', dept: 'IT' },
        { name: 'Kenwell Montekun', id: '902182', subject: 'Info Management', position: 'Professor', dept: 'Engineering' },
        { name: 'Kenwell Montekun', id: '902182', subject: 'Web Dev', position: 'Professor', dept: 'Engineering' },
        { name: 'Kenwell Montekun', id: '902182', subject: 'Network Mgmt', position: 'Professor', dept: 'Engineering' },
        { name: 'Kenwell Montekun', id: '902182', subject: 'PE', position: 'Professor', dept: 'Engineering' },
    ],
    courses: {
        BSIT: [
            { teacher: 'Moreno, Junjun', code: 'IT 210B', subject: 'Information Management, Lec.', program: 'BSME', units: 3 },
            { teacher: 'Rebese, Choyins', code: 'IT 210A', subject: 'Information Management, Lab.', program: 'BSIT', units: 3 },
            { teacher: 'Montebon, Kenwell', code: 'IT 211A', subject: 'Web Applications Dev., Lec.', program: 'BSIT', units: 3 },
            { teacher: 'Kenwell Montekun', code: 'IT 211B', subject: 'Web Applications Dev., Lab.', program: 'BSME', units: 3 },
            { teacher: 'Kenwell Montekun', code: 'IT 212B', subject: 'Network Management 2 Lec.', program: 'BSME', units: 3 },
        ],
        BSME: [
            { teacher: 'Moreno, Junjun', code: 'IT 210B', subject: 'Information Management, Lec.', program: 'BSME', units: 3 },
            { teacher: 'Rebese, Choyins', code: 'IT 210A', subject: 'Information Management, Lab.', program: 'BSIT', units: 3 },
        ]
    }
};

function getTotalStudents() {
    return window.AMS.students.BSIT.length + window.AMS.students.BSME.length;
}

function getTotalTeachers() {
    return window.AMS.teachers.length;
}

function getTotalCourses() {
    return window.AMS.courses.BSIT.length + window.AMS.courses.BSME.length;
}

function updateDashboardCounts() {
    var ds = document.getElementById('dash-students');
    var dt = document.getElementById('dash-teachers');
    var dc = document.getElementById('dash-courses');
    if (ds) ds.textContent = getTotalStudents();
    if (dt) dt.textContent = getTotalTeachers();
    if (dc) dc.textContent = getTotalCourses();
}

function renderLayout(activePage) {
    var pages = ['Dashboard', 'Student', 'Teachers', 'Courses', 'Enrollment', 'Attendance'];
    var hrefs = {
        Dashboard: '/LogIn/Dashboard',
        Student: '/LogIn/Student',
        Teachers: '/LogIn/Teacher',
        Courses: '/LogIn/Course',
        Enrollment: '/LogIn/Enrollment',
        Attendance: '/LogIn/Attendance'
    };

    var sidebar = document.getElementById('sidebar');
    if (sidebar) {
        var navLinks = '';
        for (var i = 0; i < pages.length; i++) {
            var p = pages[i];
            var cls = p === activePage ? 'active' : '';
            navLinks += '<a href="' + hrefs[p] + '" class="' + cls + '">' + p + '</a>';
        }
        sidebar.innerHTML =
            '<img src="/Assets/dbtc_LOGO.png" class="sidebar-logo" alt="DBTC Logo" />' +
            '<nav>' + navLinks + '</nav>';
    }

    var storedUser = sessionStorage.getItem('ams_user') || 'Guest';

    var topbar = document.getElementById('topbar');
    if (topbar) {
        topbar.innerHTML =
            '<div class="search-box">' +
            '<img src="/Assets/search_icon.png" style="width:16px;height:16px;opacity:0.5;" alt="search" />' +
            '<input type="text" id="globalSearch" placeholder="Search here" oninput="doSearch(this.value)" />' +
            '</div>' +
            '<div class="topbar-right">' +
            '<div style="position:relative;display:inline-block;">' +
            '<div onclick="toggleUserMenu()" style="display:flex;align-items:center;gap:8px;cursor:pointer;">' +
            '<span class="user-name">' + storedUser + '</span>' +
            '<img src="/Assets/dbtc_LOGO.png" class="avatar" style="width:36px;height:36px;border-radius:50%;object-fit:cover;border:2px solid #fff;" alt="avatar" />' +
            '</div>' +
            '<div id="userDropdown" style="display:none;position:absolute;right:0;top:48px;background:#fff;border-radius:8px;box-shadow:0 4px 16px rgba(0,0,0,0.15);min-width:160px;z-index:9999;overflow:hidden;">' +
            '<div style="padding:12px 16px;border-bottom:1px solid #eee;">' +
            '<div style="font-weight:700;font-size:0.88rem;color:#111;">' + storedUser + '</div>' +
            '<div style="font-size:0.75rem;color:#888;">Administrator</div>' +
            '</div>' +
            '<a href="/LogIn/Login" onclick="return doLogout()" style="display:block;padding:11px 16px;font-size:0.85rem;color:#e53935;text-decoration:none;font-weight:600;" onmouseover="this.style.background=\'#fff5f5\'" onmouseout="this.style.background=\'#fff\'">Logout</a>' +
            '</div>' +
            '</div>' +
            '</div>';
    }

    document.addEventListener('click', function (e) {
        var dropdown = document.getElementById('userDropdown');
        if (dropdown && !dropdown.parentElement.contains(e.target)) {
            dropdown.style.display = 'none';
        }
    });

    updateDashboardCounts();
}

function toggleUserMenu() {
    var dropdown = document.getElementById('userDropdown');
    if (dropdown) {
        dropdown.style.display = dropdown.style.display === 'none' ? 'block' : 'none';
    }
}

function doLogout() {
    if (confirm('Are you sure you want to logout?')) {
        sessionStorage.removeItem('ams_user');
        window.location.href = '/LogIn/Login';
    }
    return false;
}

function doSearch(query) {
    var q = query.toLowerCase().trim();
    var rows = document.querySelectorAll('.data-table tbody tr');
    rows.forEach(function (row) {
        var text = row.textContent.toLowerCase();
        row.style.display = (q === '' || text.includes(q)) ? '' : 'none';
    });
}