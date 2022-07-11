import os
from tqdm import tqdm
import cv2

face_directory = "Faces/"
dataset_directory = r"dataset/"
faces = os.listdir(face_directory)

if not os.path.exists(dataset_directory):
    os.mkdir(dataset_directory)

# Create folders
for i in range(33):
    if i < 9:
        dir = dataset_directory + "Person0" + str(i + 1)
    else:
        dir = dataset_directory + "Person" + str(i + 1)
    if not os.path.exists(dir):
        os.mkdir(dir)
        for j in range(8):
            for k in range(4):
                sub_dir = dir + "/" + str(j + 1) + "_" + str(k + 1)
                if not os.path.exists(sub_dir):
                    os.mkdir(dir + "/" + str(j + 1) + "_" + str(k + 1))

for i in tqdm(range(len(faces)), leave=False):
    image_name = face_directory + faces[i]

    image = cv2.imread(image_name)

    person = faces[i].split("_")[1]
    environment = faces[i].split("_")[0]
    light = faces[i].split("_")[2]

    if int(person) < 10:
        person = "0" + person

    cv2.imwrite(dataset_directory + "Person" + str(person) + "/" + str(environment) + "_" + str(light) + "/" + faces[i],
                image)
