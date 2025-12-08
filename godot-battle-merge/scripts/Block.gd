extends Node2D

## Block - Reusable block prefab with sprite and label

@onready var sprite = $Sprite2D
@onready var label = $Label

# Block colors (Neon Cyberpunk theme)
var block_colors = {
	2: Color(0.0, 1.0, 1.0),      # Cyan
	4: Color(1.0, 0.0, 1.0),      # Magenta
	8: Color(1.0, 1.0, 0.0),      # Yellow
	16: Color(0.0, 1.0, 0.0),     # Green
	32: Color(1.0, 0.5, 0.0),     # Orange
	64: Color(0.5, 0.0, 1.0),     # Purple
	128: Color(1.0, 0.0, 0.0),    # Red
	256: Color(0.0, 0.5, 1.0),    # Light Blue
	512: Color(1.0, 1.0, 1.0),    # White
	1024: Color(1.0, 0.84, 0.0),  # Gold
	2048: Color(0.5, 1.0, 0.5)    # Light Green
}

var virus_color = Color(0.8, 0.0, 0.0)  # Dark Red for viruses

func _ready():
	if not sprite:
		sprite = Sprite2D.new()
		add_child(sprite)
	if not label:
		label = Label.new()
		add_child(label)
	
	# Configure label
	label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	label.vertical_alignment = VERTICAL_ALIGNMENT_CENTER
	label.position = Vector2(-40, -20)
	label.size = Vector2(80, 40)
	
	# Add a simple colored square as sprite (will be replaced with actual sprites)
	_create_square_texture()

func _create_square_texture():
	"""Create a simple colored square texture"""
	# Create a ColorRect as visual representation
	var color_rect = ColorRect.new()
	color_rect.size = Vector2(90, 90)
	color_rect.position = Vector2(-45, -45)
	color_rect.color = Color(0.2, 0.2, 0.2)  # Default dark gray
	
	# Remove old sprite if it exists
	if sprite and sprite.get_parent():
		sprite.queue_free()
	
	sprite = color_rect
	add_child(sprite)
	move_child(sprite, 0)  # Behind label

func set_value(value: int, is_virus: bool = false):
	"""Set the block value and update appearance"""
	# Update label
	if label:
		label.text = str(value)
		
		# Configure label style
		if label.get_theme_font_size("font_size") == -1:
			label.add_theme_font_size_override("font_size", 24)
		label.add_theme_color_override("font_color", Color.WHITE)
	
	# Update color
	if sprite and sprite is ColorRect:
		if is_virus:
			sprite.color = virus_color
		elif block_colors.has(value):
			sprite.color = block_colors[value]
		else:
			# Default color for values not in dictionary
			sprite.color = Color(0.5, 0.5, 0.5)
		
		# Add glow effect for neon theme
		var glow_intensity = 0.3
		sprite.material = _create_glow_material(sprite.color, glow_intensity)

func _create_glow_material(base_color: Color, intensity: float) -> Material:
	"""Create a material with a glow effect"""
	var material = CanvasItemMaterial.new()
	material.blend_mode = CanvasItemMaterial.BLEND_MODE_ADD
	# Note: For true glow, we'd need a shader. This is a simplified version.
	return null  # Return null for now, will use base colors
