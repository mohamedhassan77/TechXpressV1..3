document.addEventListener('DOMContentLoaded', function () {
    // Sidebar Toggle
    const sidebarToggle = document.querySelector('.sidebar-toggle');
    const wrapper = document.querySelector('.wrapper');

    sidebarToggle.addEventListener('click', () => {
        wrapper.classList.toggle('sidebar-active');
    });

    // Notification Bell
    const notificationBell = document.querySelector('.btn-notification');
    notificationBell.addEventListener('click', () => {
        // Handle notification click
    });

    // Responsive Tables
    document.querySelectorAll('.table-responsive').forEach(table => {
        let isDown = false;
        let startX;
        let scrollLeft;

        table.addEventListener('mousedown', (e) => {
            isDown = true;
            startX = e.pageX - table.offsetLeft;
            scrollLeft = table.scrollLeft;
        });

        table.addEventListener('mouseleave', () => {
            isDown = false;
        });

        table.addEventListener('mouseup', () => {
            isDown = false;
        });

        table.addEventListener('mousemove', (e) => {
            if (!isDown) return;
            e.preventDefault();
            const x = e.pageX - table.offsetLeft;
            const walk = (x - startX) * 2;
            table.scrollLeft = scrollLeft - walk;
        });
    });
});