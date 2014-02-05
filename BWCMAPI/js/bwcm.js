$(window).load(function() {
$('.page').hide();
$('#pageContainer').removeClass('hidden'); // only used while loading

var xhr = new XMLHttpRequest();
if (!xhr.upload) {
	alert("Browser does not support background image uploads, please use a modern browser.");
}

var loggedOut = false;
$.ajaxSetup({
	cache: false,
	error: function(xhr, textStatus, error) {
		if (xhr.status == 403) {
			if (!loggedOut) {
				loggedOut = true;
				alert("Session has expired -- you are now logged out.");
				window.location.href = '/index.html';
			}
		} else {
			if (!loggedOut)
				alert("Server error: " + textStatus + ": " + error);
		}
	}
});

// on startup
refreshUser();

$('#login').click(function() {
	var user = $('#loginUser').val(), pass = $('#loginPass').val();
	$.get('/api/login', {user: user, pass: pass}, function(resp) {
		$('#loginPass').val('')
		refreshUser();
		if (resp.error) {
			$('#loginError').text(resp.error);
		}
	});
});
$('#loginPass').keyup(function(e) { if (e.keyCode == 13) $('#login').click(); });
$('#logout').click(function() {
	$.get('/api/logout', function(resp) {
		refreshUser();
	});
});
$('a[data-page]').click(function() {
	page($(this).data('page'));
});

}); // window.load

function page(name, quiet) {
	var p = $('.page[data-page="' + name + '"]');
	if (!p.length) {
		alert("unkown page name: " + name);
		return;
	}
	
	if (p.is(':visible')) return; // already shown
	$('.page').hide();
	p.show();
	
	// li highlight
	$('a[data-page]').parents('li').removeClass('active');
	$('a[data-page="' + name + '"]').parents('li').addClass('active');
	
	if (quiet) return;
	
	switch (name) {
		case 'login':
			$('#loginUser').focus();
			break;
		case 'crawl':
			refreshCrawl();
			break;
		case 'users':
			refreshUserTable();
			break;
		case 'players':
			refreshPlayerTable();
			break;
		case 'player':
			refreshPlayer();
			break;
	}
}

function refreshUser() {
	$('#loginError').text('');
	$('#userLoggedIn').hide();
	
    $.get('/api/info', function(resp) {
		$('#startupLoader').hide();
		user = resp.user;
		$('.admin').toggle((user != null) && user.isAdmin);
        if (user == null) {
			page('login');
            return;
        }
		if (resp.user.isAdmin) {
			page('users');
        } else {
			page('players');
        }
		$('#userLoggedIn').show();
        $('#userText').text(user.user);
    });
}

function refreshCrawl() {
	$('#crawlLoading').show(); $('#crawlText').hide();
	$.get('/api/crawl', function(resp) {
		$('#crawlText').val(resp.result);
		$('#remainingText').text((5000 - resp.result.length) + ' characters remaining.');
		$('#crawlLoading').hide(); $('#crawlText').show();
	});
}

$(document).on('input', '#crawlText', function() {
	var text = $('#crawlText').val();
	$('#remainingText').text((5000 - text.length) + ' characters remaining.');
});

$('#updateCrawl').click(function() {
	$.get('/api/crawl/update', {crawl: $('#crawlText').val()}, function() {
		$('#updateCrawl').text('Saved!');
		setTimeout(function() { $('#updateCrawl').text('Save Changes'); }, 2000);
	});
});

$('#changePasswd').click(function() {
	var p1 = $('#passwd1').val(), p2 = $('#passwd2').val();
	if (!p1) return;
	if (p1 != p2) {
		alert("Passwords must match.");
		return;
	}
	$.get('/api/passwd', {pass: p1}, function() {
		$('#changePasswd').text('Password changed.');
		setTimeout(function() { $('#changePasswd').text('Change Password'); }, 2000);
	});
});