'''
Combine the Poly Haven diffuse and roughness textures into Unity's URP albedo texture.
'''

from PIL import Image
import sys
import os

def merge_images(rgb_path, alpha_path, output_path):
    # Open the images
    rgb_image = Image.open(rgb_path)
    alpha_image = Image.open(alpha_path)

    # Ensure both images have the same size
    if rgb_image.size != alpha_image.size:
        print("Error: Images must be the same size")
        return

    # # Convert the images to RGBA mode
    # rgb_image = rgb_image.convert('RGBA')
    # alpha_image = alpha_image.convert('L')

    # # Create a new RGBA image
    # merged_image = Image.new('RGBA', rgb_image.size)

    # # Merge the RGB channels from the first image and alpha channel from the second image
    # merged_image.paste(rgb_image, (0, 0), rgb_image)
    # merged_image.putalpha(alpha_image)

    # # Save the merged image
    # merged_image.save(output_path)
    alpha_image = alpha_image.convert('L')
    alpha_image = Image.eval(alpha_image, lambda x: 255 - x)
    merged_image = rgb_image.convert('RGBA')
    merged_image.putalpha(alpha_image)
    merged_image.save(output_path)
    
    os.remove(rgb_path)
    os.remove(alpha_path)

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python merge_images.py <texture_name>")
        sys.exit(1)
    
    image_files = []
    for file in os.listdir('.'):
        if file.lower().endswith(('.png', '.jpg', '.jpeg', '.bmp', 'tga')):
            image_files.append(file)
    
    texName = sys.argv[1]
    rgb_path = ''
    suffix = ''
    for file in image_files:
        if file.startswith(texName + '_diff'):
            rgb_path = file
            suffix, _ = os.path.splitext(file)
            suffix = suffix.lstrip(texName + '_diff')
            break
    alpha_path = ''
    for file in image_files:
        if file.startswith(texName + '_rough'):
            alpha_path = file
            break
    if rgb_path == '':
        print(f"Cannot find {texName + '_diff'}.")
        sys.exit(1)
    if alpha_path == '':
        print(f"Cannot find {texName + '_rough'}.")
        sys.exit(1)
    output_path = texName + '_albedo_smooth' + suffix + '.tga'
    merge_images(rgb_path, alpha_path, output_path)
