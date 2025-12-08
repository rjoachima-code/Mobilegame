extends Node2D

## Battle Merge - Main Game Controller
## Implements M2 Block merge mechanics with lane defense

# Constants
const GRID_COLS = 5
const GRID_ROWS = 8
const CELL_SIZE = 100
const GRID_OFFSET = Vector2(110, 150)  # Offset from top-left for grid positioning

# Grid state
var grid = []  # 5x8 2D array: grid[col][row]

# Game state
var current_block_value = 2
var next_block_value = 2
var score = 0
var coins = 0
var game_over = false
var virus_spawn_timer = 0.0
const VIRUS_SPAWN_INTERVAL = 10.0

# Block visuals
var block_scene = preload("res://scenes/Block.tscn")
var block_instances = {}  # Dictionary: "col_row" -> Block node

# Economy configuration
var economy = {
	"swap_cost": 50,
	"sledgehammer_cost": 100,
	"joker_cost": 250,
	"nuke_cost": 500
}

# Item state
var next_is_joker = false

# UI references (to be set in _ready or via editor)
@onready var score_label = $UI/ScoreLabel
@onready var coins_label = $UI/CoinsLabel
@onready var current_block_label = $UI/CurrentBlockLabel
@onready var next_block_label = $UI/NextBlockLabel
@onready var game_over_panel = $UI/GameOverPanel

func _ready():
	# Initialize grid
	grid.clear()
	for col in range(GRID_COLS):
		grid.append([])
		for row in range(GRID_ROWS):
			grid[col].append(0)
	
	# Initialize game state
	_generate_next_block()
	current_block_value = next_block_value
	_generate_next_block()
	
	# Update UI
	_update_ui()
	
	print("Battle Merge initialized. Grid: ", GRID_COLS, "x", GRID_ROWS)

func _process(delta):
	if game_over:
		return
	
	# Update virus spawn timer
	virus_spawn_timer += delta
	if virus_spawn_timer >= VIRUS_SPAWN_INTERVAL:
		virus_spawn_timer = 0.0
		_spawn_viruses()

func _input(event):
	if game_over:
		return
	
	if event is InputEventScreenTouch and event.pressed:
		_handle_screen_touch(event.position)
	elif event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
		_handle_screen_touch(event.position)

func _handle_screen_touch(position: Vector2):
	# Convert screen position to grid column
	var local_pos = position - GRID_OFFSET
	if local_pos.x < 0 or local_pos.y < 0:
		return
	
	var col_index = int(local_pos.x / CELL_SIZE)
	if col_index < 0 or col_index >= GRID_COLS:
		return
	
	handle_column_tap(col_index)

## SECTION 1: CORE M2 BLOCK MECHANIC

func handle_column_tap(col_index: int):
	"""Primary player action - tap a column to drop the current block"""
	if game_over:
		return
	
	# Find lowest empty spot in column
	var target_row = _find_lowest_empty(col_index)
	
	# Check if column is full
	if target_row < 0:
		_trigger_game_over("Column full!")
		return
	
	# Place the block
	var value_to_place = current_block_value
	if next_is_joker:
		# Joker becomes the value it lands on for guaranteed merge
		if target_row < GRID_ROWS - 1 and grid[col_index][target_row + 1] > 0:
			value_to_place = grid[col_index][target_row + 1]
		next_is_joker = false
	
	grid[col_index][target_row] = value_to_place
	_create_block_visual(col_index, target_row, value_to_place)
	
	# Check for merge
	_check_merge(col_index, target_row)
	
	# Apply gravity after merges
	_apply_gravity(col_index)
	
	# Move to next block
	current_block_value = next_block_value
	_generate_next_block()
	
	# Virus mechanics (defense layer)
	_descend_viruses()
	
	# Update UI
	_update_ui()

func _find_lowest_empty(col: int) -> int:
	"""Find the lowest empty row in a column, returns -1 if full"""
	for row in range(GRID_ROWS - 1, -1, -1):
		if grid[col][row] == 0:
			return row
	return -1

func _check_merge(col: int, row: int):
	"""Recursive vertical merge check"""
	if row >= GRID_ROWS - 1:
		return  # Bottom row, nothing below
	
	var current_value = grid[col][row]
	var below_value = grid[col][row + 1]
	
	# Check if values match and can merge
	if current_value == below_value and current_value > 0 and below_value > 0:
		# Merge: double the value in lower cell
		var new_value = below_value * 2
		grid[col][row + 1] = new_value
		grid[col][row] = 0
		
		# Update visuals
		_remove_block_visual(col, row)
		_update_block_visual(col, row + 1, new_value)
		
		# Calculate coin payout
		_calculate_coin_payout(new_value, false)
		
		# Play merge animation
		_play_merge_animation(col, row + 1)
		
		# Recursive merge check
		_check_merge(col, row + 1)

func _apply_gravity(col: int):
	"""Apply gravity to a column - slide blocks down to fill empty spaces"""
	var changed = true
	while changed:
		changed = false
		for row in range(GRID_ROWS - 1, 0, -1):
			if grid[col][row] == 0 and grid[col][row - 1] > 0:
				# Move block down
				grid[col][row] = grid[col][row - 1]
				grid[col][row - 1] = 0
				
				# Update visuals
				_move_block_visual(col, row - 1, col, row)
				changed = true

## SECTION 2: DEFENSE LAYER (VIRUS THREAT)

func _spawn_viruses():
	"""Spawn virus blocks at the top of random columns"""
	var num_viruses = randi_range(1, 2)  # Spawn 1-2 viruses
	
	for i in range(num_viruses):
		var col = randi_range(0, GRID_COLS - 1)
		
		# Check if top row (row 0) is empty
		if grid[col][0] == 0:
			var virus_value = [2, 4][randi() % 2]  # Random 2 or 4
			grid[col][0] = -virus_value  # Negative values represent viruses
			_create_block_visual(col, 0, -virus_value, true)
			print("Virus spawned: col=", col, " value=", virus_value)

func _descend_viruses():
	"""Move all virus blocks down one row after player turn"""
	# Process from bottom to top to avoid double-moving
	for col in range(GRID_COLS):
		for row in range(GRID_ROWS - 1, -1, -1):
			if grid[col][row] < 0:  # Virus (negative value)
				var virus_value = grid[col][row]
				
				# Check if at bottom
				if row == GRID_ROWS - 1:
					_trigger_game_over("Virus reached the bottom!")
					return
				
				# Check what's below
				var below_value = grid[col][row + 1]
				
				if below_value == 0:
					# Empty space - descend
					grid[col][row + 1] = virus_value
					grid[col][row] = 0
					_move_block_visual(col, row, col, row + 1)
				elif below_value > 0:
					# Defense interaction
					_defense_interaction(col, row + 1, below_value, -virus_value)
					# Remove virus
					grid[col][row] = 0
					_remove_block_visual(col, row)

func _defense_interaction(col: int, row: int, block_value: int, virus_value: int):
	"""Handle block vs virus interaction"""
	if block_value >= virus_value:
		# Block wins - virus destroyed, block remains
		print("Block defends! Block:", block_value, " >= Virus:", virus_value)
		_play_defense_animation(col, row)
	else:
		# Virus wins - both destroyed
		print("Virus destroys block! Virus:", virus_value, " > Block:", block_value)
		grid[col][row] = 0
		_remove_block_visual(col, row)

## SECTION 3: ECONOMY & ITEMS

func _calculate_coin_payout(new_value: int, is_chain: bool):
	"""Calculate coins earned from a merge"""
	# Find n where 2^n = new_value
	var n = log(new_value) / log(2)
	
	# Base payout: ceil(n/3) * 2
	var base_payout = ceil(n / 3.0) * 2
	
	# Chain bonus: multiply by 2 if part of chain
	if is_chain:
		base_payout *= 2
	
	coins += int(base_payout)
	score += int(new_value)  # Also add to score

func use_item_swap():
	"""Item: Swap current and next block values"""
	if coins < economy.swap_cost:
		print("Not enough coins for Swap")
		return false
	
	coins -= economy.swap_cost
	var temp = current_block_value
	current_block_value = next_block_value
	next_block_value = temp
	_update_ui()
	print("Swapped blocks")
	return true

func use_item_sledgehammer(col: int, row: int):
	"""Item: Remove a single block"""
	if coins < economy.sledgehammer_cost:
		print("Not enough coins for Sledgehammer")
		return false
	
	if col < 0 or col >= GRID_COLS or row < 0 or row >= GRID_ROWS:
		return false
	
	if grid[col][row] == 0:
		print("No block at that position")
		return false
	
	coins -= economy.sledgehammer_cost
	grid[col][row] = 0
	_remove_block_visual(col, row)
	_apply_gravity(col)
	_update_ui()
	print("Sledgehammer used at ", col, ",", row)
	return true

func use_item_joker():
	"""Item: Next block becomes a Joker"""
	if coins < economy.joker_cost:
		print("Not enough coins for Joker")
		return false
	
	coins -= economy.joker_cost
	next_is_joker = true
	_update_ui()
	print("Next block is now a Joker")
	return true

func use_item_column_nuke(col: int):
	"""Item: Clear all blocks in a column"""
	if coins < economy.nuke_cost:
		print("Not enough coins for Column Nuke")
		return false
	
	if col < 0 or col >= GRID_COLS:
		return false
	
	coins -= economy.nuke_cost
	for row in range(GRID_ROWS):
		if grid[col][row] != 0:
			_remove_block_visual(col, row)
			grid[col][row] = 0
	_update_ui()
	print("Column ", col, " nuked")
	return true

## HELPER FUNCTIONS

func _generate_next_block():
	"""Generate the next block value (2, 4, or 8)"""
	var rand = randf()
	if rand < 0.6:
		next_block_value = 2
	elif rand < 0.9:
		next_block_value = 4
	else:
		next_block_value = 8

func _create_block_visual(col: int, row: int, value: int, is_virus: bool = false):
	"""Create a visual block instance"""
	var block = block_scene.instantiate()
	var key = "%d_%d" % [col, row]
	
	# Position the block
	var pos = GRID_OFFSET + Vector2(col * CELL_SIZE + CELL_SIZE/2, row * CELL_SIZE + CELL_SIZE/2)
	block.position = pos
	
	# Set block properties
	if block.has_method("set_value"):
		block.set_value(abs(value), is_virus)
	
	add_child(block)
	block_instances[key] = block

func _update_block_visual(col: int, row: int, value: int):
	"""Update an existing block visual"""
	var key = "%d_%d" % [col, row]
	if block_instances.has(key):
		var block = block_instances[key]
		if block.has_method("set_value"):
			block.set_value(value, false)

func _remove_block_visual(col: int, row: int):
	"""Remove a block visual"""
	var key = "%d_%d" % [col, row]
	if block_instances.has(key):
		var block = block_instances[key]
		block.queue_free()
		block_instances.erase(key)

func _move_block_visual(from_col: int, from_row: int, to_col: int, to_row: int):
	"""Move a block visual from one position to another"""
	var from_key = "%d_%d" % [from_col, from_row]
	var to_key = "%d_%d" % [to_col, to_row]
	
	if block_instances.has(from_key):
		var block = block_instances[from_key]
		var new_pos = GRID_OFFSET + Vector2(to_col * CELL_SIZE + CELL_SIZE/2, to_row * CELL_SIZE + CELL_SIZE/2)
		
		# Animate movement
		var tween = create_tween()
		tween.tween_property(block, "position", new_pos, 0.2)
		
		# Update dictionary
		block_instances[to_key] = block
		block_instances.erase(from_key)

func _play_merge_animation(col: int, row: int):
	"""Play merge animation (screen shake + particles)"""
	# Simple screen shake
	var tween = create_tween()
	var original_pos = position
	tween.tween_property(self, "position", original_pos + Vector2(5, 0), 0.05)
	tween.tween_property(self, "position", original_pos - Vector2(5, 0), 0.05)
	tween.tween_property(self, "position", original_pos, 0.05)
	
	# TODO: Add particle effect at merge location

func _play_defense_animation(col: int, row: int):
	"""Play defense animation (laser effect)"""
	# TODO: Add laser sprite shooting upward from block
	pass

func _update_ui():
	"""Update all UI elements"""
	if score_label:
		score_label.text = "Score: %d" % score
	if coins_label:
		coins_label.text = "Coins: %d" % coins
	if current_block_label:
		if next_is_joker:
			current_block_label.text = "Current: JOKER"
		else:
			current_block_label.text = "Current: %d" % current_block_value
	if next_block_label:
		next_block_label.text = "Next: %d" % next_block_value

func _trigger_game_over(reason: String):
	"""Trigger game over state"""
	game_over = true
	print("GAME OVER: ", reason)
	if game_over_panel:
		game_over_panel.visible = true
	# TODO: Show game over UI with final score

func restart_game():
	"""Restart the game"""
	# Clear all blocks
	for key in block_instances.keys():
		block_instances[key].queue_free()
	block_instances.clear()
	
	# Reset grid
	for col in range(GRID_COLS):
		for row in range(GRID_ROWS):
			grid[col][row] = 0
	
	# Reset state
	score = 0
	coins = 0
	game_over = false
	virus_spawn_timer = 0.0
	next_is_joker = false
	
	_generate_next_block()
	current_block_value = next_block_value
	_generate_next_block()
	
	if game_over_panel:
		game_over_panel.visible = false
	
	_update_ui()
	print("Game restarted")
