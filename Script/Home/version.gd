extends Label


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	var version = str(ProjectSettings.get_setting("application/config/version"))
	var branch = str(ProjectSettings.get_setting("application/config/branch"))
	if version == '0.0.0':
		text = 'Development Build (Qlute)'
	else:
		text = 'Qlute/' + branch + ' ('+ version + '-' + branch + ')'
