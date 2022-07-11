import cv2
from tqdm import tqdm
import os
import json
import re


def sorted_alphanumeric(dir):
    """
    Function that sorts the files of a directory when loaded with os.listdir
    :param dir: Directory
    :return: Sorted directory
    """

    def convert(text): return int(text) if text.isdigit() else text.lower()

    def alphanum_key(key): return [convert(c) for c in re.split('([0-9]+)', key)]

    return sorted(dir, key=alphanum_key)


image_directory = r"F:\Perception Outputs\7299ee8a-0fa5-475f-9a0c-99adce51c605\RGBImages/"
json_directory = r"F:\Perception Outputs\7299ee8a-0fa5-475f-9a0c-99adce51c605\Dataset Data/"

# Sorting the directories
images = sorted_alphanumeric(os.listdir(image_directory))
json_files = sorted_alphanumeric(os.listdir(json_directory))

list_of_x = []
list_of_y = []
list_of_width = []
list_of_height = []

# Annotation definition of the 2D Bounding Box Labeler
bb2d_definition = "f9f22e05-443f-4602-a422-ebe4ea9b55cb"

# Creating the folder that will contain the face images
if not os.path.exists("Faces"):
    os.mkdir("Faces")

# Getting the 2D bounding box coordinates and dimensions of the heads of each image
for i in tqdm(range(len(json_files)), leave=False):
    filename = json_directory + json_files[i]
    if "captures" in filename:
        f = open(filename)
        data = json.load(f)
        for j in range(len(data['captures'])):
            capture = data['captures'][j]
            for k in range(len(capture['annotations'])):
                if capture['annotations'][k]['annotation_definition'] == bb2d_definition:
                    annotation = capture['annotations'][k]
                    for l in range(len(annotation['values'])):
                        value = annotation['values'][l]
                        if value['label_name'] == 'head':
                            list_of_x.append(int(value['x']))
                            list_of_y.append(int(value['y']))
                            list_of_width.append(int(value['width']))
                            list_of_height.append(int(value['height']))

# Cropping the images in order to retain only the heads
for i in tqdm(range(len(images)), leave=False):
    image_name = image_directory + images[i]
    image = cv2.imread(image_name)

    x_start = list_of_y[i]
    x_end = x_start + list_of_height[i]
    x_total = x_end - x_start

    y_start = list_of_x[i]
    y_end = y_start + list_of_width[i]
    y_total = y_end - y_start

    # Squaring the images
    start_x = True
    while x_total < y_total:
        if start_x:
            x_start -= 1
        else:
            x_end += 1
        x_total = x_end - x_start
        start_x = not start_x

    start_y = True
    while y_total < x_total:
        if start_y:
            y_start -= 1
        else:
            y_end += 1
        y_total = y_end - y_start
        start_y = not start_y

    cropped_image = image[x_start:x_end, y_start:y_end, :]

    cv2.imwrite("Faces/" + images[i], cropped_image)
