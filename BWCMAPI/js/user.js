var user;
var users, editUser = null;

function refreshUserTable() {
	$('#userTable').html('<img src="ajax-loader.gif">');
	$.get('/api/users', function(r) {
		users = r.result;
		users = users.sort(function(a,b) { if (a.user < b.user) return -1; if (a.user > b.user) return 1; return 0; });
		$('#userTable').html('');
		$('#userTable').json2html(users, userRow);
	});
	$.get('/api/players', function(r) {
		var players = r.result;
		players = players.sort(function(a,b) { if (a.name < b.name) return -1; if (a.name > b.name) return 1; return 0; });
		$('#userPlayers').html('');
		$('#userPlayers').json2html(players, playerOption);
	});
}

$(window).load(function() {
	
$(document).on('click', '#userTable .edit', function() {
	var u = $(this).parents('tr').data('user');
	for (var i = 0; i < users.length; i++) {
		if (users[i].user == u) {
			editUser = users[i];
			$('#userName').val(editUser.user);
			$('#userName').attr('disabled', true);
			$('#userAdmin').prop('checked', editUser.isAdmin);
			$('#userPlayersGroup').toggle(!editUser.isAdmin);
			$('#userPlayers').val(editUser.players);
			
			$('#userEdit').modal();
			break;
		}
	}
});

$('#userAdmin').change(function() {
	$('#userPlayersGroup').toggle(!$('#userAdmin').is(':checked'));
});

$('#userAdd').click(function() {
	editUser = null;
	$('#userName').val('');
	$('#userName').attr('disabled', false);
	$('#userPass').val('');
	$('#userEdit').modal();
});

$('#userSave').click(function() {
	var user, pass, players, isAdmin;
	if (editUser == null) {
		editUser = {__type: 'user', user: $('#userName').val(), password: $('#userPass').val()};
	} else {
		if ($('#userPass').val())
			editUser.password = $('#userPass').val(); // change password
	}
	editUser.isAdmin = $('#userAdmin').is(':checked');
	if (editUser.isAdmin) {
		editUser.players = [];
	} else {
		editUser.players = $('#userPlayers').val();
	}
	$('#userPass').val('');
	$.post('/api/users/update', JSON.stringify(editUser), function(r) {
		console.log(r);
		$('#userEdit').modal('hide');
		refreshUserTable();
	}).fail(function(r) {
		alert('fail');
		console.log(r);
	});
});
		
$('#userDelete').click(function() {
	if (editUser == null || !confirm("Really delete user " + editUser.user + "?")) {
		// nothing to do
		$('#userEdit').modal('hide');
		return;
	}
	$.get('/api/user/delete', {user: editUser.user}, function(r) {
		console.log(r);
		$('#userEdit').modal('hide');
		refreshUserTable();
	});
});

}); // window.load

// templates
var userRow = {tag: 'tr', 'data-user': '${user}', children: [
  {tag: 'td', html: '${user}'},
  {tag: 'td', html: function() {
	if (this.isAdmin) return 'all (administrator)';
	return this.players.join(', ');
	}},
  {tag: 'td', html: '<button class="btn btn-primary edit">Edit</button>'}
]};

var playerOption = {tag: 'option', 'value': '${name}', html: '${name}'};
