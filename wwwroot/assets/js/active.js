
document.addEventListener("DOMContentLoaded", function () {
    const currentLocation = window.location.pathname; // Lấy URL hiện tại
    const menuItems = document.querySelectorAll(".navbar-nav .nav-link");

    menuItems.forEach(link => {
        const linkPath = link.getAttribute("href");

        // So sánh link với URL hiện tại (bỏ qua dấu '/')
        if (currentLocation.includes(linkPath) && linkPath !== "/") {
            menuItems.forEach(l => l.classList.remove("active")); // bỏ active cũ
            link.classList.add("active"); // thêm active mới
        }
        // Nếu ở trang chủ
        if (currentLocation === "/" && linkPath === "/") {
            link.classList.add("active");
        }
    });
});
