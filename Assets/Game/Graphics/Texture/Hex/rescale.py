import os
from PIL import Image
import math

# Define your folder path
folder_path = "./swamp"  # Change this to your actual folder path

# Define scale factors
width_scale = 1.0
height_scale = math.sqrt(3) / 2  # ~0.866

# Loop through files in the folder
for filename in os.listdir(folder_path):
    if filename.lower().endswith(".png"):
        image_path = os.path.join(folder_path, filename)

        try:
            with Image.open(image_path) as img:
                # Get original dimensions
                original_width, original_height = img.size

                # Compute new dimensions
                new_width = int(original_width * width_scale)
                new_height = int(original_height * height_scale)

                # Resize image
                resized_img = img.resize((new_width, new_height), Image.Resampling.LANCZOS)

                # Save (overwrite original or save as new file)
                resized_img.save(image_path)
                print(f"Resized {filename} to {new_width}x{new_height}")
        except Exception as e:
            print(f"Failed to process {filename}: {e}")
