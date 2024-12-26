import re

# Function to update the pivot value for each chunk
def update_pivot(chunk, some_value):
    # Find the pivot x and y
    pivot_pattern = r'pivot: {x: ([^,]+), y: ([^}]+)}'
    match = re.search(pivot_pattern, chunk)
    
    if match:
        current_x_value = float(match.group(1))
        current_y_value = float(match.group(2))
        
        # Find the height value
        height_pattern = r'rect:\s+serializedVersion: \d+\s+x: \d+\s+y: \d+\s+width: \d+\s+height: (\d+)'
        height_match = re.search(height_pattern, chunk)
        
        if height_match:
            current_height = float(height_match.group(1))
            # Check if current_height is zero to avoid division by zero
            if current_height == 0:
                print("Warning: Found rect with height 0. Skipping pivot update for this chunk.")
                return chunk  # You can return the chunk unchanged, or handle it differently
                
            # Compute the new pivot.y value
            new_pivot_y = current_y_value + (some_value / current_height)
            
            # Create the new pivot string
            new_pivot_str = f"pivot: {{x: {current_x_value}, y: {new_pivot_y}}}"
            
            # Replace the old pivot value with the new one
            updated_chunk = re.sub(pivot_pattern, new_pivot_str, chunk)
            return updated_chunk
    return chunk

# Function to process the whole file
def process_file(input_file, output_file, some_value):
    with open(input_file, 'r') as f:
        content = f.read()
    
    # Define the pattern to extract each chunk of relevant data
    chunk_pattern = r'(pivot: {[^}]+}\s+alignment: \d+\s+border: {[^}]+}\s+customData: \s+rect:\s+serializedVersion: \d+\s+x: \d+\s+y: \d+\s+width: \d+\s+height: \d+)'
    
    # Find all chunks that need modification
    chunks = re.findall(chunk_pattern, content)
    
    for chunk in chunks:
        updated_chunk = update_pivot(chunk, some_value)
        content = content.replace(chunk, updated_chunk)
    
    # Write the updated content to the output file
    with open(output_file, 'w') as f:
        f.write(content)

# Main function
if __name__ == '__main__':
    input_file = 'final_goku.aseprite.meta'  # Path to your input file
    output_file = 'final_goku.aseprite.meta1'  # Path to the output file
    some_value = 72  # Define the value you want to use for the update (this is the "some_value_i_define")
    
    # Process the file
    process_file(input_file, output_file, some_value)
    print(f"File processed and saved as {output_file}")
