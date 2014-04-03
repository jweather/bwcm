var players, editPlayer = null, editSlide = null, editSlideIndex, slideEditClosing = false;
var playerDirty = false;
var templates, templateByID;

var refreshPlayer, refreshPlayerTable;

$(window).load(function() {

refreshPlayerTable = function refreshPlayerTable() {
	$('#playerTable').html('<img src="ajax-loader.gif">');
	$.get('/api/players', function(r) {
		players = r.result;
		players = players.sort(function(a,b) {
			if (a.name < b.name) return -1; if (a.name > b.name) return 1; return 0; 
		});
		players.id = {};
		for (var i = 0; i < players.length; i++) {
			players.id[players[i].id] = players[i];
		}
			
		$('#playerTable').html('');
		$('#playerTable').json2html(players, playerRow);
	});
	$.get('/api/templates', function(r) {
		templates = r.result;
		templateByID = {};
		templates = templates.sort(function(a,b) {
			if (a.name < b.name) return -1; if (a.name > b.name) return 1; return 0; 
		});
		for (var i = 0; i < templates.length; i++) {
			var t = templates[i];
			templateByID[templates[i].id] = t;
			t.fieldByName = {};
			for (var fi = 0; fi < t.fields.length; fi++)
				t.fieldByName[t.fields[fi].name] = t.fields[fi];
			if (t.info != null && t.info.fields != null) {
				for (var fi = 0; fi < t.info.fields.length; fi++) {
					var finfo = t.info.fields[fi];
					t.fieldByName[finfo.n].info = finfo;
				}
			}
		}
		try { $('#slideTemplate').ddslick('destroy'); } catch (err) {}
		$('#slideTemplate').html('');
		$('#slideTemplate').json2html(templates, templateOption);
		$('#slideTemplate').ddslick({
			width: 532, 
			imagePosition: 'left', 
			selectText: 'Select a slide template',
			onSelected: function (data) {
				if (editSlide) {
					editSlide.templateID = data.selectedData.value;
					templateChanged();
				}
			}
		});
	});
}

refreshPlayer = function refreshPlayer() {
	$('#playerTitle').html('Editing ' + editPlayer.name + ' playlist:');
	
	for (var s=0; s < editPlayer.slides.length; s++)
		editPlayer.slides[s].index = s;
	$('#slideTable').html('');
	slidesExpired = [];
	$('#slideTable').json2html(editPlayer.slides, slideRow);
	$('.deleteExpired').toggle(slidesExpired.length > 0);
	$('#slideTable').sortable({
		handle: '.slideThumb',
		stop: function() {
			$('#playerSave').show();
			playerDirty = true;
			var slides = editPlayer.slides;
			editPlayer.slides = [];
			var i = 0;
			$('#slideTable tr').each(function() {
				var index = $(this).data('index');
				$(this).data('index', i++);
				editPlayer.slides.push(slides[index]);
			});
		}
	});
}

$(document).on('click', '#playerTable .edit', function() {
	var id = $(this).parents('tr').data('id');
	editPlayer = players.id[id];
	page('player');
	$('#playerSave').hide();
});

$(document).on('click', '#playerTable .settings', function() {
	var id = $(this).parents('tr').data('id');
	editPlayer = players.id[id];
	$('#playerInfoName').text('Settings for ' + editPlayer.name);
	$('#playerInfoTime').val(editPlayer.info.defaultDuration);
	$('#playerInfoTwitter').val(editPlayer.info.defaultTwitter);
	page('playerInfo');
});

$('#playerInfoSave').click(function() {
	$('#playerInfoSave').text('Saving...');
	var time = parseInt($('#playerInfoTime').val(), 10);
	if (!time) {
		alert('Please enter a number larger than 0 for the default slide time');
		return;
	}
	editPlayer.info.defaultDuration = time;
	editPlayer.info.defaultTwitter = $('#playerInfoTwitter').val();
	
	$.post('/api/players/update', JSON.stringify(editPlayer), function(r) {
		editPlayer = r.result;
		page('players');
		$('#playerInfoSave').text('Save Changes');
	}).fail(function(r) {
		console.log(r);
	});
});

$('#playerInfoNukeTime').click(function() {
	var time = parseInt($('#playerInfoTime').val(), 10);
	if (!time) {
		alert('Please enter a number larger than 0 for the default slide time');
		return;
	}

	if (!confirm('Really set duration of ' + time + ' seconds for ALL slides on this player?')) return;

	editPlayer.info.defaultDuration = time;
	for (var i = 0; i < editPlayer.slides.length; i++) {
		editPlayer.slides[i].duration = time;
		editPlayer.slides[i].changed = true;
	}
	page('player', true);
	playerSave();
});

$('#playerInfoNukeTwitter').click(function() {
	var handles = $('#playerInfoTwitter').val();
	if (!handles) {
		alert('Please enter one or more Twitter handles to use');
		return;
	}

	if (!confirm('Really set Twitter handles to ' + handles + ' for ALL slides on this player?')) return;
	
	editPlayer.info.defaultTwitter = handles;
	for (var i = 0; i < editPlayer.slides.length; i++) {
		var slide = editPlayer.slides[i];
		for (var f = 0; f < slide.fields.length; f++) {
			if (slide.fields[f].widget && slide.fields[f].widget.__type == 'twitter') {
				slide.fields[f].widget.handles = handles;
				slide.changed = true;
			}
		}
	}
	page('player', true);
	playerSave();
});

function beginEditSlide() {
	editSlide.fieldByName = {};
	for (var j = 0; j < editSlide.fields.length; j++) {
		editSlide.fieldByName[editSlide.fields[j].name] = editSlide.fields[j];
	}
	$('#slideName').val(editSlide.name);
	var wasDirty = editSlide.changed;
	$('#slideTemplate').ddslick('select', {value: editSlide.templateID});
	templateChanged();
	editSlide.changed = wasDirty; // template change always marks slide dirty
	
	if (editSlide.startDate == null) {
		$('#slideStartDate').datepicker('setValue', '');
		$('#slideStartDate').val('');
	} else {
		$('#slideStartDate').datepicker('setValue', editSlide.startDate);
	}
	
	if (editSlide.stopDate == null) {
		$('#slideStopDate').datepicker('setValue', '');
		$('#slideStopDate').val('');
	} else {
		$('#slideStopDate').datepicker('setValue', editSlide.stopDate);
	}
	$('#slideStartTime').val(editSlide.startTime == '0:00' ? '' : editSlide.startTime);
	$('#slideStopTime').val(editSlide.stopTime == '24:00' ? '' : editSlide.stopTime);
	$('#slideDuration').val(editSlide.duration || editPlayer.info.defaultDuration);
	
	$('.days :checkbox').each(function() {
		$(this).prop('checked', editSlide.days == null || editSlide.days.indexOf($(this).val()) != -1);
	});
	
	$('#slideEdit').modal();
}

$(document).on('click', '#slideTable .edit', function() {
	editSlideIndex = $(this).parents('tr').data('index');
	editSlide = clone(editPlayer.slides[editSlideIndex]);

	beginEditSlide();
});

$(document).on('click', '#slideTable .copy', function() {
	editSlideIndex = $(this).parents('tr').data('index');
	editSlide = clone(editPlayer.slides[editSlideIndex]);
	editSlideIndex = -1; // save as new slide
	delete editSlide.id;
	editSlide.changed = true;

	beginEditSlide();
});

$(document).on('click', '#slideTable .thumbEnlarge', function() {
	var src = $(this).data('src');
	$('#thumbZoomImg').attr('src', src);
	$('#thumbZoom').modal();
});



$('.slideAdd').click(function() {
	editSlide = {__type: 'slide', fields: [], fieldByName: {}};
	editSlideIndex = -1;
	$('#slideName').val('New Slide');
	$('#slideDuration').val(editPlayer.info.defaultDuration);
	
	$('#slideTemplate').ddslick('clear');
	editSlide.templateID = null;
	templateChanged();

	$('#slideEdit').modal();
	editSlide.changed = false;
});

$('.deleteExpired').click(function() {
	if (!confirm('Really delete ' + slidesExpired.length + ' expired slides?')) return;

	editPlayer.slides = editPlayer.slides.filter(function(s) { return slidesExpired.indexOf(s.id) == -1; });
	playerDirty = true;
	playerSave();
});

$(document).on('input', '#slideName,#slideStartDate,#slideStopDate,#slideStartTime,#slideStopTime', function() {
	editSlide.changed = true;
});

$('#slideStartDate,#slideStopDate').datepicker().on('changeDate', function() {
	editSlide.changed = true;
});

$('.days :checkbox').click(function() {
	editSlide.changed = true;
});

function templateChanged() {
	editSlide.changed = true;
	var tid = editSlide.templateID;
	var t = templateByID[tid];
	$('#slideFields').html('');
	if (tid == null) return;
	
	$('#slideFields').json2html(t.fields, templateField);
	$('#slideFields .fieldRow').each(function() {
		var fieldName = $(this).data('name');
		var valueType = null;

		var field = editSlide.fieldByName[fieldName];
		if (!editSlide.fieldByName[fieldName]) {
			field = {name: fieldName};
			editSlide.fields.push(field);
			editSlide.fieldByName[fieldName] = field;
		}
		
		// hide tabs that don't apply
		var allowed = [];
		var tf = t.fieldByName[fieldName];
		if (tf.info && tf.info.ws)
			allowed = allowed.concat(tf.info.ws);
		$(this).find('.nav-tabs a').each(function() {
			var widget = $(this).data('widget');
			if (allowed.indexOf(widget) == -1)
				$(this).hide();
		});
		
		if (field.widget) {
			valueType = field.widget.__type;
			if (valueType == 'text') {
				$('#' + fieldName + '-textValue').val(field.widget.text);
			} else if (valueType == 'twitter') {
				$('#' + fieldName + '-twitterValue').val(field.widget.handles);
			}
		} else {
			valueType = allowed[0];
			field.widget = {__type: valueType};
			if (allowed.indexOf('twitter') >= 0 && editPlayer.info.defaultTwitter) {
				valueType = 'twitter';
				field.widget = {__type: 'twitter', handles: editPlayer.info.defaultTwitter};
				$('#' + fieldName + '-twitterValue').val(editPlayer.info.defaultTwitter);
			}
		}
		field.widget.W = tf.info.w; field.widget.H = tf.info.h;
		

		// show active tab
		$('#slideEdit a[href="#' + fieldName + '-' + valueType + '"]').tab('show');
		previewWidget(fieldName);
	});
}

// change field type
$(document).on('click', '#slideEdit .nav-tabs a', function(e) {
	var fieldName = $(this).parents('.fieldRow').data('name');
	var field = editSlide.fieldByName[fieldName];
	var widget = $(this).data('widget');
	field.widget['__type'] = widget;

	editSlide.changed = true;
	e.preventDefault();
	$(this).tab('show');
	
	previewWidget(fieldName);
});

var uploadedDataFor = {}
$('#slideEdit').on('hide.bs.modal', function(e) {
	if (e.target != this) return; // datepicker is throwing hide events too
	if (slideEditClosing) return;
	slideEditClosing = true;
	if (editSlide && editSlide.changed) {
		var yes = confirm("Close without saving?");
		if (!yes) {
			slideEditClosing = false;
			return e.preventDefault();
		}
	} else {
		editSlide = null;
	}
});

$('#slideEdit').on('hidden.bs.modal', function() {
	uploadedDataFor = {};
	slideEditClosing = false;
});

function previewWidget(fieldName) {
	var field = editSlide.fieldByName[fieldName];
	var finfo = templateByID[editSlide.templateID].fieldByName[fieldName].info;
	var widget = $('#field-' + fieldName + ' .nav-tabs li.active a').data('widget');
	var preview = $('#preview-' + fieldName);
	var src = '';
	if (widget == 'image') {
		if (uploadedDataFor[fieldName])
			src = uploadedDataFor[fieldName];
		else {
			// generate an appropriate placeholder image using the desired width/height of the image
			src = '/thumbnail.aspx?id=' + field.mediaID + '&placeholder=' + finfo.w + 'x' + finfo.h;
		}
	} else if (widget == 'none') {
		src = '';
	} else {
		if (!field.widget) {
			field.widget = {__type: widget};
		}
		src = '/widget.aspx/preview?widget=' + encodeURIComponent(JSON.stringify(field.widget));
	}
	$(preview).attr('src', src);
}

var widgetPreview = {};
$(document).on('input', '.twitterValue, .textValue', function() {
	editSlide.changed = true;
	var fieldName = $(this).parents('.fieldRow').data('name');
	var field = editSlide.fieldByName[fieldName];
	var widget = $('#field-' + fieldName + ' .nav-tabs li.active a').data('widget');
	if (widget == 'text') {
		field.widget.text = $('#' + fieldName + '-textValue').val();
	} else if (widget == 'twitter') {
		field.widget.handles = $('#' + fieldName + '-twitterValue').val();
	}
	
	// reset timeout to refresh widget preview
	if (widgetPreview[fieldName])
		clearInterval(widgetPreview[fieldName]);
	widgetPreview[fieldName] = setTimeout(function() { previewWidget(fieldName); }, 500);
});

$(document).on('change', '#slideEdit input[type=file]', function(e) {
	editSlide.changed = true;
	var files = e.target.files || e.dataTransfer.files;
	var file = files[0];
	var fieldName = $(e.target).data('name');
	if (file.type.indexOf("image") == 0) {
		$('#uploading-' + fieldName).show();
	
		var reader = new FileReader();
		reader.onload = function(e) {
			var preview = $('#preview-' + fieldName);
			uploadedDataFor[fieldName] = e.target.result;
			previewWidget(fieldName);
		}
		reader.readAsDataURL(file);
		var xhr = new XMLHttpRequest();
		if (!editSlide.uploads) editSlide.uploads = {};
		editSlide.uploads[fieldName] = xhr;
		xhr.open('POST', '/api/upload', true);
		xhr.setRequestHeader('X_FILENAME', file.name);
		xhr.setRequestHeader('X_PLAYER', editPlayer.name);
		xhr.onload = function(e) {
			if (!editSlide) return;
			if (xhr.status == 403) {
				alert("Session has expired -- you are now logged out");
				window.location.href = '/index.html';
				return;
			} else if (xhr.status != 200) {
				alert("Server error, upload failed");
			} else {
				var r = JSON.parse(e.target.response);
				console.log('uploaded new media item with ID ' + r.id + ' for field ' + fieldName);
				editSlide.fieldByName[fieldName].mediaID = r.id;
				delete editSlide.uploads[fieldName];
				$('#uploading-' + fieldName).hide();
			}
		}
		xhr.onerror = function(xhr) {
			alert("Network error, upload failed");
			delete editSlide.uploads[fieldName];
		}
		xhr.send(file);
	}
});

function validate(val) {
	if (!val) return null;
	val = val.trim();
	if (val == '') return null;
	var matches = val.match(/\d{1,2}\/\d{1,2}(\/\d{2,4})?/);
	if (matches == null || val != matches[0]) return false;
	if (matches[1] == undefined) 
		return val + '/' + new Date().getFullYear();
	return val;
}

function valitime(val) {
	val = val.trim();
	if (val == '') return null;
	var matches = val.match(/(\d{1,2})(:\d\d)? ?(am|pm|AM|PM)?/);
	if (matches == null || val != matches[0]) return false;
	var h = parseInt(matches[1],10);
	if (matches[3] && matches[3].toLowerCase() == 'pm') {
		if (h < 12)	h += 12;
	}
	if (matches[3] && matches[3].toLowerCase() == 'am') {
		if (h == 12) h = 0;
	}
	if (h < 0 || h > 23) return false;
	var m = matches[2] ? parseInt(matches[2].slice(1),10) : 0;
	if (m > 59) return false;
	return ('0' + h).slice(-2) + ':' + ('0' + m).slice(-2);
}

$('#slideSave').click(function() {
	if (editSlide.templateID == null) {
		alert("Please select a template and assign fields before saving.");
		return;
	}
	
	if (editSlide.uploads) {
		for (u in editSlide.uploads) {
			alert("Please wait for uploads to finish before saving.");
			return;
		}
	}
	
	editSlide.startDate = validate($('#slideStartDate').val());
	editSlide.stopDate = validate($('#slideStopDate').val());
	if (editSlide.startDate == false || editSlide.stopDate == false) {
		alert("Please enter dates like 03/22/2004.");
		return;
	}
	
	// time range is disabled
	editSlide.startTime = null; //valitime($('#slideStartTime').val());
	editSlide.stopTime = null; //valitime($('#slideStopTime').val());
	if (editSlide.startTime == false || editSlide.stopTime == false) {
		alert("Please enter times like 13:00 or 1:00 pm");
		return;
	}
	
	// days are disabled
	//editSlide.days = $('.days :checkbox:checked').map(function() { return $(this).val(); }).toArray();
	
	var duration = parseInt($('#slideDuration').val(), 10);
	if (!duration)
		return alert('Please enter the slide time as a number of seconds larger than 0');
	if (duration != editSlide.duration)
		editSlide.changed = true;
	editSlide.duration = duration;

	var ok = true;
	$('#slideFields .fieldRow').each(function() {
		var fieldName = $(this).data('name');
		var field = editSlide.fieldByName[fieldName];
		
		if (field.widget.__type == 'image') {
			if (field.mediaID) return;
		} else if (field.widget.__type == 'text') {
			if (field.widget.text) return;
		} else if (field.widget.__type == 'twitter') {
			if (field.widget.handles) return;
		} else { // weather or none
			return;
		}

		ok = false;
		alert("Please assign a value to each field before saving.");
		return false;
	});
	if (!ok) return;
	
	editSlide.name = $('#slideName').val();
	
	// commit changes to local structure
	if (editSlideIndex == -1) {
		editPlayer.slides.push(editSlide);
	} else {
		editPlayer.slides[editSlideIndex] = editSlide;
	}
	
	slideEditClosing = true; // don't prompt about changes
	$('#slideEdit').modal('hide');
	playerDirty = true;
	editSlide = null;
	playerSave();
});

$('#slideDelete').click(function() {
	if (editSlide.id == null) {
		$('#slideEdit').modal('hide');
		return; // never existed
	}
	editPlayer.slides.splice(editSlideIndex, 1);
	editSlide = null;
	playerDirty = true;
	$('#slideEdit').modal('hide');
	playerSave();
});

$('#playerSave').click(function() { playerSave(); });

function playerSave() {
	$('#slideTable').html('<img src="ajax-loader.gif">');
	$.post('/api/players/update', JSON.stringify(editPlayer), function(r) {
		editPlayer = r.result;
		setTimeout(refreshPlayer, 1000); // update slide list to show new thumbnail URL
		playerDirty = false;
		$('#playerSave').hide();
	}).fail(function(r) {
		console.log(r);
	});
}

}); // window.load

// templates
var playerRow = {tag: 'tr', 'data-id': '${id}', children: [
  {tag: 'td', width: '60%', html: '${name}'},
  {tag: 'td', width: '40%', children: [
	{tag: 'button', 'class': 'btn btn-primary edit', html: 'Edit'},
	{tag: 'button', 'class': 'btn settings', html: 'Edit Default Settings'}
  ]}
]};

function titleCase(str) {
	if (!str || str.length == 0) return '';
	if (str.length == 1) return str.toUpperCase();
	return str.substr(0, 1).toUpperCase() + str.substr(1).toLowerCase();
}

var slidesExpired = [];
var slideRow = {tag: 'tr', 'data-index': '${index}', children: [
  {tag: 'td', width: 244, children: [
	{tag: 'img', 'class': 'slideThumb', src: '/thumbnail.aspx?id=${id}'},
	{tag: 'img', 'class': 'thumbEnlarge', src: 'mag.png', 'data-src': '/thumbnail.aspx?id=${id}&big=1'}
	]},
  {tag: 'td', html: function() {
	var lines = ['<b>' + this.name + '</b>'];
	if (this.startDate) lines.push('Starting on ' + this.startDate);
	if (this.stopDate) {
		var expired = '';
		if (new Date(this.stopDate) < new Date()-1000*60*60*24) {
			expired = ' <font color="red">Expired</font>';
			slidesExpired.push(this.id);
		}
		lines.push('Ending on ' + this.stopDate + expired);
	}
	if (this.startTime) lines.push('From ' + this.startTime + ' to ' + this.stopTime);
	if (this.days && this.days.length < 7) {
		lines.push('On ' + this.days.map(function(d) { return titleCase(d); }).join(', '));
	}
	if (this.duration != editPlayer.info.defaultDuration)
		lines.push('Play for ' + this.duration + ' seconds');
	return lines.join('<br>');
	}},
  {tag: 'td', children: [
	{tag: 'button', 'class': 'btn btn-primary edit', html: 'Edit'},
	{tag: 'button', 'class': 'btn copy', html: 'Copy'}
	]}
]};

var templateOption = {tag: 'option', 'value': '${id}', html: '${name}',
	'data-imagesrc': '/thumbnail.aspx?id=${id}'};

var templateField = {tag: 'div', 'class': 'form-group', children: [
	{tag: 'label', 'for': 'field-${name}', html: '${name}'},
	{tag: 'div', id: 'field-${name}', 'data-name': '${name}', 'class': 'form-control fieldRow', children: [
		{tag: 'div', 'class': 'fieldPreview', children: [
			{tag: 'img', id: 'preview-${name}', 'data-name': '${name}'},
			{tag: 'div', html: 'Uploading...', 'class': 'uploading', id: 'uploading-${name}'}]},
		{tag: 'div', 'class': 'fieldValues', children: [
			{tag: 'ul', 'class': 'nav nav-tabs', children: [
				{tag: 'li', 'class': 'image', children: 
					[{tag: 'a', href: '#${name}-image', 'data-toggle': 'tab', html: 'Image', 'data-widget': 'image'}]},
				{tag: 'li', 'class': 'text', children: 
					[{tag: 'a', href: '#${name}-text', 'data-toggle': 'tab', html: 'Text', 'data-widget': 'text'}]},
				{tag: 'li', 'class': 'twitter', children: 
					[{tag: 'a', href: '#${name}-twitter', 'data-toggle': 'tab', html: 'Twitter', 'data-widget': 'twitter'}]},
				{tag: 'li', 'class': 'weather', children: 
					[{tag: 'a', href: '#${name}-weather', 'data-toggle': 'tab', html: 'Weather', 'data-widget': 'weather'}]},
				{tag: 'li', 'class': 'none', children: 
					[{tag: 'a', href: '#${name}-none', 'data-toggle': 'tab', html: 'None', 'data-widget': 'none'}]}
			]},
			{tag: 'div', 'class': 'tab-content', children: [
				{tag: 'div', 'class': 'tab-pane', id: '${name}-image', children: [
					{tag: 'p', html: 'Select an image file to upload:'},
					{tag: 'input', type: 'file', id: 'upload-${name}', 'data-name': '${name}'}
				]},
				{tag: 'div', 'class': 'tab-pane', id: '${name}-text', children: [
					{tag: 'p', html: 'Enter text to display:'},
					{tag: 'textarea', id: '${name}-textValue', 'class': 'textValue',
						rows: 4, cols: 40}
				]},
				{tag: 'div', 'class': 'tab-pane', id: '${name}-twitter', children: [
					{tag: 'p', html: 'Enter twitter handles to follow:'},
					{tag: 'input', type: 'text', id: '${name}-twitterValue', 'class': 'twitterValue',
						size: 40, placeholder: '(ex: @bwathletics @bwadmission)'}
				]},
				{tag: 'div', 'class': 'tab-pane', id: '${name}-weather', children: [
					{tag: 'p', html: 'Shows current weather conditions.'}
				]},
				{tag: 'div', 'class': 'tab-pane', id: '${name}-none', children: [
					{tag: 'p', html: 'No content.'}
				]}
			]},
		]}, // fieldValues
		{tag: 'div', 'class': 'fieldClear'}
	]} // field-${name}
]};

function clone(from, to) {
    if (from == null || typeof from != "object") return from;
    if (from.constructor != Object && from.constructor != Array) return from;
    if (from.constructor == Date || from.constructor == RegExp || from.constructor == Function ||
        from.constructor == String || from.constructor == Number || from.constructor == Boolean)
        return new from.constructor(from);

    to = to || new from.constructor();

    for (var name in from)
    {
        to[name] = typeof to[name] == "undefined" ? clone(from[name], null) : to[name];
    }

    return to;
}
