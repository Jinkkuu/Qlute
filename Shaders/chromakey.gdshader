shader_type canvas_item;

// Uniform variables for chroma key effect
uniform vec3 chroma_key_color : source_color = vec3(0.0, 1.0, 0.0);
uniform float pickup_range : hint_range(0.0, 1.0) = 0.1;
uniform float fade_amount : hint_range(0.0, 1.0) = 0.1;

void fragment() {
	// Get the color from the texture at the given UV coordinates
	vec4 color = texture(TEXTURE, UV);
	// Calculate the distance between the current color and the chroma key color
	// the lesser the distance more likely the colors are
	float distance = length(color.rgb - chroma_key_color);
	// If the distance is within the pickup range, discard the pixel
	if (distance <= pickup_range) {
		discard;
	}
	// Calculate the fade factor based on the pickup range and fade amount
	float fade_factor = smoothstep(pickup_range, pickup_range + fade_amount, distance);
	// Set the output color with the original RGB values and the calculated fade factor
	COLOR = vec4(color.rgb, fade_factor);
}
