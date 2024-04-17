//document.addEventListener("DOMContentLoaded", function (event) {

//    const showNavbar = (toggleId, navId, bodyId, headerId) => {
//        const toggle = document.getElementById(toggleId),
//            nav = document.getElementById(navId),
//            bodypd = document.getElementById(bodyId),
//            headerpd = document.getElementById(headerId)

//        if (toggle && nav && bodypd && headerpd) {
//            toggle.addEventListener('click', () => {
//                // show navbar
//                nav.classList.toggle('show')
//                // change icon
//                toggle.classList.toggle('bx-x')
//                // add padding to body
//                bodypd.classList.toggle('body-pd')
//                // add padding to header
//                headerpd.classList.toggle('body-pd')
//            })
//        }
//    }
//document.addEventListener("DOMContentLoaded", function (event) {
//    console.log('start')
//    // Prevent default behavior for all links within the nav_link class
//    const navLinks = document.querySelectorAll('.nav_link');

//    navLinks.forEach(link => {
//        link.addEventListener('click', function (event) {
//            // Prevent default link behavior
//            event.preventDefault();

//            // Check if the link has asp-action, asp-controller, and asp-area attributes
//            if (this.hasAttribute('asp-action') && this.hasAttribute('asp-controller')) {
//                let url = '';

//                // Get URL components from attributes
//                const action = this.getAttribute('asp-action');
//                const controller = this.getAttribute('asp-controller');
//                const area = this.hasAttribute('asp-area') ? this.getAttribute('asp-area') : ''; // Optional area

//                // Construct the URL using the base URL and components
//                url = `${window.location.origin}/${area ? area + '/' : ''} ${controller}/${action}`;

//                // Manually trigger navigation using window.location.href
//                window.location.href = url;
//            }
//        });
//    });

//    // Your code to run since DOM is loaded and ready
//});

//showNavbar('header-toggle', 'nav-bar', 'body-pd', 'header')

//const linkColor = document.querySelectorAll('.nav_link')

//function colorLink() {
//    if (linkColor) {
//        linkColor.forEach(l => l.classList.remove('active'))
//        this.classList.add('active')
//    }
//}
//    linkColor.forEach(l => l.addEventListener('click', colorLink)){

//    }

//    // Your code to run since DOM is loaded and ready
