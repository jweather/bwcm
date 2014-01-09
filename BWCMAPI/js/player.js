var players, editPlayer = null, editSlide = null, editSlideIndex;
var playerDirty = false;
var templates, templateByID;

function refreshPlayerTable() {
	$('#playerTable').html('<img src="ajax-loader.gif">');
	$.get('/api/players', function(r) {
		players = r.result;
		players = players.sort(function(a,b) {
			if (a.name < b.name) return -1; if (a.name > b.name) return 1; return 0; 
		});
		$('#playerTable').html('');
		$('#playerTable').json2html(players, playerRow);
	});
	$.get('/api/templates', function(r) {
		templates = r.result;
		templateByID = {};
		for (var i = 0; i < templates.length; i++) {
			var t = templates[i];
			templateByID[templates[i].id] = t;
			t.fieldByName = {};
			for (var fi = 0; fi < t.fields.length; fi++)
				t.fieldByName[t.fields[fi].name] = t.fields[fi];
			if (t.info != null && t.info.fields != null) {
				for (var fi = 0; fi < t.info.fields.length; fi++) {
					var finfo = t.info.fields[fi];
					t.fieldByName[finfo.name].info = finfo;
				}
			}
		}
		$('#slideTemplate').html('');
		$('#slideTemplate').json2html(templates, templateOption);
	});
}

function refreshPlayer() {
	$('#playerTitle').html('Editing ' + editPlayer.name + ' playlist:');
	
	for (var s=0; s < editPlayer.slides.length; s++)
		editPlayer.slides[s].index = s;
	$('#slideTable').html('');
	$('#slideTable').json2html(editPlayer.slides, slideRow);
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

$(window).load(function() {

$(document).on('click', '#playerTable .edit', function() {
	var id = $(this).parents('tr').data('id');
	for (var i = 0; i < players.length; i++) {
		if (players[i].id == id) {
			editPlayer = players[i];
			page('player');
			$('#playerSave').hide();
		}
	}
});

$(document).on('click', '#slideTable .edit', function() {
	editSlideIndex = $(this).parents('tr').data('index');
	editSlide = clone(editPlayer.slides[editSlideIndex]);
	editSlide.fieldByName = {};
	for (var j = 0; j < editSlide.fields.length; j++) {
		editSlide.fieldByName[editSlide.fields[j].name] = editSlide.fields[j];
	}
	$('#slideName').val(editSlide.name);
	var wasDirty = editSlide.changed;
	$('#slideTemplate').val(editSlide.templateID).change();
	editSlide.changed = wasDirty; // template change always marks slide dirty
	
	$('#slideEdit').modal();
});

$('#slideAdd').click(function() {
	editSlide = {__type: 'slide', fields: [], fieldByName: {}};
	editSlideIndex = -1;
	$('#slideName').val('New Slide');
	$('#slideTemplate').val('').change();

	$('#slideEdit').modal();
	editSlide.changed = true;
});

$(document).on('input', '#slideName', function() {
	editSlide.changed = true;
});

$('#slideTemplate').change(function() {
	editSlide.changed = true;
	var tid = $('#slideTemplate').val();
	editSlide.templateID = tid;
	var t = templateByID[tid];
	$('#slideFields').html('');
	if (tid == null) return;
	$('#slideFields').json2html(t.fields, templateField);
	$('#slideFields .fieldRow').each(function() {
		var fieldName = $(this).data('name');
		var valueType = 'image';

		var field = editSlide.fieldByName[fieldName];
		if (!editSlide.fieldByName[fieldName]) {
			field = {name: fieldName};
			editSlide.fields.push(field);
			editSlide.fieldByName[fieldName] = field;
		}
		
		if (field.widget) {
			valueType = field.widget.__type;
			if (valueType == 'text') {
				$('#' + fieldName + '-textValue').val(field.widget.text);
			} else if (valueType == 'twitter') {
				$('#' + fieldName + '-twitterValue').val(field.widget.handles);
			}
		}

		// hide tabs that don't apply
		var allowed = ['image']; // always allowed?
		var tf = t.fieldByName[fieldName];
		if (tf.info && tf.info.widgets)
			allowed = allowed.concat(tf.info.widgets);
		$(this).find('.nav-tabs a').each(function() {
			var widget = $(this).data('widget');
			if (allowed.indexOf(widget) == -1)
				$(this).hide();
		});
		
		// show active tab
		$('#slideEdit a[href="#' + fieldName + '-' + valueType + '"]').tab('show');
		previewWidget(fieldName);
	});
});

// change field type
$(document).on('click', '#slideEdit .nav-tabs a', function(e) {
	editSlide.changed = true;
	e.preventDefault();
	$(this).tab('show');
	var widget = $(this).data('widget');
	var field = $(this).parents('.fieldRow').data('name');
	previewWidget(field);
});

var uploadedDataFor = {}
var slideEditClosing = false;
$('#slideEdit').on('hide.bs.modal', function() {
	if (slideEditClosing) return;
	slideEditClosing = true;
	if (editSlide && editSlide.changed) {
		var yes = confirm("Save changes before closing?");
		if (!yes) return;
		$('#slideSave').click();
	} else {
		editSlide = null;
	}
});

$('#slideEdit').on('hidden.bs.modal', function() {
	uploadedDataFor = {};
	slideEditClosing = false;
});

function saveWidget(fieldName) {
	var field = editSlide.fieldByName[fieldName];
	var widget = $('#field-' + fieldName + ' .nav-tabs li.active a').data('widget');
	if (widget == 'image') {
		field.widget = null;
		return;
	}
	
	field.widget = {__type: widget};
	if (widget == 'text') {
		field.widget.text = $('#' + fieldName + '-textValue').val();
	} else if (widget == 'twitter') {
		field.widget.handles = $('#' + fieldName + '-twitterValue').val();
	}
}

function previewWidget(fieldName) {
	var field = editSlide.fieldByName[fieldName];
	var widget = $('#field-' + fieldName + ' .nav-tabs li.active a').data('widget');
	var preview = $('#preview-' + fieldName);
	var src = '';
	if (widget == 'image') {
		if (uploadedDataFor[fieldName])
			src = uploadedDataFor[fieldName];
		else {
			// generate an appropriate placeholder image using the desired width/height of the image
			var finfo = templateByID[editSlide.templateID].fieldByName[fieldName].info;
			src = '/thumbnail.aspx?id=' + field.mediaID + '&placeholder=' + finfo.width + 'x' + finfo.height;
		}
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
	var name = $(this).parents('.fieldRow').data('name');
	// reset timeout to refresh widget preview
	if (widgetPreview[name])
		clearInterval(widgetPreview[name]);
	widgetPreview[name] = setTimeout(function() { saveWidget(name); previewWidget(name); }, 500);
});

$(document).on('change', '#slideEdit input[type=file]', function(e) {
	editSlide.changed = true;
	var files = e.target.files || e.dataTransfer.files;
	var file = files[0];
	var fieldName = $(e.target).data('name');
	if (file.type.indexOf("image") == 0) {
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
			}
		}
		xhr.onerror = function(xhr) {
			alert("Network error, upload failed");
			delete editSlide.uploads[fieldName];
		}
		xhr.send(file);
	}
});

$('#slideSave').click(function() {
	if ($('#slideTemplate').val() == null) {
		alert("Please select a template and assign fields before saving.");
		return;
	}
	
	if (editSlide.uploads) {
		for (u in editSlide.uploads) {
			alert("Please wait for uploads to finish before saving.");
			return;
		}
	}
	
	var ok = true;
	$('#slideFields .fieldRow').each(function() {
		var fieldName = $(this).data('name');
		var field = editSlide.fieldByName[fieldName];
		
		if (!field.mediaID && !field.widget) {
			ok = false;
			alert("Please assign a value to each field before saving.");
			return false;
		}
	});
	if (!ok) return;
	
	editSlide.name = $('#slideName').val();
	// fields have already been set
	
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
  {tag: 'td', html: function() { return this.name.replace('/@/g', ''); }},
  {tag: 'td', html: '<button class="btn btn-primary edit">Edit</button>'}
]};

var slideRow = {tag: 'tr', 'data-index': '${index}', children: [
  {tag: 'td', width: 244, children: [
	{tag: 'img', 'class': 'slideThumb', src: '/thumbnail.aspx?id=${id}'}]},
  {tag: 'td', html: '${name}'},
  {tag: 'td', html: '<button class="btn btn-primary edit">Edit</button>'}
]};

var templateOption = {tag: 'option', 'value': '${id}', html: '${name}'};

var templateField = {tag: 'div', 'class': 'form-group', children: [
	{tag: 'label', 'for': 'field-${name}', html: '${name}'},
	{tag: 'div', id: 'field-${name}', 'data-name': '${name}', 'class': 'form-control fieldRow', children: [
		{tag: 'div', 'class': 'fieldPreview', children: [
			{tag: 'img', id: 'preview-${name}', 'data-name': '${name}'}]},
		{tag: 'div', 'class': 'fieldValues', children: [
			{tag: 'ul', 'class': 'nav nav-tabs', children: [
				{tag: 'li', 'class': 'image', children: 
					[{tag: 'a', href: '#${name}-image', 'data-toggle': 'tab', html: 'Image', 'data-widget': 'image'}]},
				{tag: 'li', 'class': 'text', children: 
					[{tag: 'a', href: '#${name}-text', 'data-toggle': 'tab', html: 'Text', 'data-widget': 'text'}]},
				{tag: 'li', 'class': 'twitter', children: 
					[{tag: 'a', href: '#${name}-twitter', 'data-toggle': 'tab', html: 'Twitter', 'data-widget': 'twitter'}]},
				{tag: 'li', 'class': 'weather', children: 
					[{tag: 'a', href: '#${name}-weather', 'data-toggle': 'tab', html: 'Weather', 'data-widget': 'weather'}]}
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