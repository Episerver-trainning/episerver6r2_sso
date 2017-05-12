if (navigator.userAgent.match(/Android/i) || navigator.userAgent.match(/iPhone/i)) {
    $(document).ready(function() {
        $('#MainMenu').append('<div class="leftButton" onclick="toggleMenu()">Menu</div>');
        $('#MainMenu ul, #Functions ul, #SubMenuArea ul').addClass('hide');
    });
    function toggleMenu() {
        $('#MainMenu .leftButton').toggleClass('pressed');
        $('#MainMenu ul, #Functions ul, #SubMenuArea ul').toggleClass('hide');
    }
}