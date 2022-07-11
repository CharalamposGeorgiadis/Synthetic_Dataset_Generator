# Synthetic_Dataset_Generator
The aim of this Unity project is to generate synthetic datasets for training and evaluating , mainly but not limited to, Active Perception/Vision methods. It utilises a modified version 0.9.0-preview.1 of the [Unity Perception Package](https://github.com/Unity-Technologies/com.unity.perception) along with some additional scripts and Randomizers that were developed for the purpose of this work.

This project was used in my diploma thesis (and is set up in such a way) for generating a synthetic dataset for every valid combination of 8 environments, 33 humans, 4 light conditions, 7 camera distances (1m - 4m away from the human) and 36 camera angles (0° - 360° with 10° increments, rotating around the human). A combination is rendered invalid if camera GameObject is colliding with another object. The project also supports the addition of animations to the humans with two Randomizers that control the changing of the animations and the proper playing of each one. 

The main dataset generation algorithm can be described by a sequence of nested for-loops:

```
For each Environment e
  For each Human h
    For each Lighting Condition l
      For each Animation a
        For each Animation Frame f
          For each Camera Position p
            For each Camera Angle a
              Capture and output images and metadata
```

## Image Examples
For each valid capture, the project generates an RGB image and its semantic segmentation counterpart.

### RGB Images
![Images](https://user-images.githubusercontent.com/72664246/177645117-ee686ab1-d043-44ab-a06a-1fc8c44d96f6.jpg)

### Semantic Segmentation Images
![Semantic](https://user-images.githubusercontent.com/72664246/177645984-ee049d24-fd7e-451b-a547-849ee49bfd28.png)

## Generated Folders
The Unity project generates 3 folder:
- Datset Data: Contains the JSON files that are generated during the Simulation.
- RGBImages: Contains the RGB images.
- SemanticSegmentationImages: Contains the semantic segmentation images.

### Dataset Data
During the Simulation, the Perception package generates JSON files which contain ground truth annotations that describe each captured image. The package's source code was modified so that each JSON file contains annotations for one environment, one human and one lighting condition. If animations are enabled, each JSON file contains annotations for one environment, one human, four lighting conditions and one animation.

The dataset that was generated in my diploma thesis contained 2D Bounding Box, 3D Bounding Box and Keypoint annotations.

### Image Naming
If animations are enabled, each image is named: *e_h_l_a_f_d_r.jpg*, where
-  **e** denotes the id of the environment.
-  **h** denotes the id of the human.
-  **l** denotes the id of the lighting condition.
-  **a** denotes the id of the animation.
-  **f** denotes the frame of the animation.
-  **d** denotes the camera's distance from the human.
-  **r** denotes the rotation degrees of the camera.

If animations are enabled, each image is named: *e_h_l_d_r.jpg*.

## Used Assets
The Unity project that was developed for my diploma thesis included 8 environments created from free assets downloaded from the [Unity Asset Store](https://assetstore.unity.com/?gclid=Cj0KCQjw5ZSWBhCVARIsALERCvw1Bhpyz7oRJ-wyDHO-6OJuqiU-nU1S0uTIDNy_6Mbz9tNTsrmLGsIaAuUrEALw_wcB&gclsrc=aw.ds). 29 out of the 33 human models were downloaded from the [Asset Store](https://assetstore.unity.com/?gclid=Cj0KCQjw5ZSWBhCVARIsALERCvw1Bhpyz7oRJ-wyDHO-6OJuqiU-nU1S0uTIDNy_6Mbz9tNTsrmLGsIaAuUrEALw_wcB&gclsrc=aw.ds), [Mixamo](https://www.mixamo.com/#/) and [Turbosquid](https://www.turbosquid.com/?&utm_source=google&utm_medium=cpc&utm_campaign=RoEUAF-en-TS-Brand&utm_content=ts%20brand&utm_term=turbosquid&mt=e&dev=c&itemid=&targid=kwd-297496938642&loc=9061579&ntwk=g&dmod=&adp=&gclid=Cj0KCQjw5ZSWBhCVARIsALERCvx-98mKydP7qVEzkzbkv1eKZioniGXh6Mx24qUdCa4lmnYCegmD8H0aAlvpEALw_wcB&gclsrc=aw.ds). Due to obvious legal reasons, these assets are not included in this repository, however the [Asset Store](https://assetstore.unity.com/?gclid=Cj0KCQjw5ZSWBhCVARIsALERCvw1Bhpyz7oRJ-wyDHO-6OJuqiU-nU1S0uTIDNy_6Mbz9tNTsrmLGsIaAuUrEALw_wcB&gclsrc=aw.ds) and [Turbosquid](https://www.turbosquid.com/?&utm_source=google&utm_medium=cpc&utm_campaign=RoEUAF-en-TS-Brand&utm_content=ts%20brand&utm_term=turbosquid&mt=e&dev=c&itemid=&targid=kwd-297496938642&loc=9061579&ntwk=g&dmod=&adp=&gclid=Cj0KCQjw5ZSWBhCVARIsALERCvx-98mKydP7qVEzkzbkv1eKZioniGXh6Mx24qUdCa4lmnYCegmD8H0aAlvpEALw_wcB&gclsrc=aw.ds) assets and models that were used are linked below:

### Environments
- [Apartment Kit](https://assetstore.unity.com/packages/3d/props/apartment-kit-124055)
- [Classic Picture Frame](https://assetstore.unity.com/packages/3d/props/furniture/classic-picture-frame-59038)
- [Dining Set](https://assetstore.unity.com/packages/3d/props/interior/dining-set-37029)
- [HQ Laptop Computer](https://assetstore.unity.com/packages/3d/props/electronics/hq-laptop-computer-42030)
- [Lamp Model](https://assetstore.unity.com/packages/3d/props/interior/lamp-model-110960)
- [Office Room Furniture](https://assetstore.unity.com/packages/3d/props/furniture/office-room-furniture-70884)
- [Old "USSR" Lamp](https://assetstore.unity.com/packages/3d/props/electronics/old-ussr-lamp-110400)
- [Pack Gesta Furniture \#1](https://assetstore.unity.com/packages/3d/props/furniture/pack-gesta-furniture-1-28237)
- [PBR Electronics Pack](https://assetstore.unity.com/packages/3d/props/electronics/pbr-electronics-pack-38741)
- [Toon Furniture](https://assetstore.unity.com/packages/3d/props/furniture/toon-furniture-88740)
- [TV Furniture](https://assetstore.unity.com/packages/3d/props/electronics/tv-furniture-60122)

### Humans
- [3D Claudia Rigged 002](https://www.turbosquid.com/3d-models/3d-photorealistic-human-rig-1422551)
- [Carla Rigged 001 3D](https://www.turbosquid.com/3d-models/photorealistic-human-rig-3d-1422548)
- [Eric Rigged 001 3D](https://www.turbosquid.com/3d-models/photorealistic-human-rig-3d-1422553)
- [Man in a Suit](https://assetstore.unity.com/packages/3d/characters/humanoids/humans/man-in-a-suit-51662)

Four additional humans (one male adult, one male baby, one female adult, one female baby) were designed using [MakeHuman](http://www.makehumancommunity.org). Since, I created these models, they are included in this repository in the *Assets/MyModels* folder.

# Important Notes
- When opening the project, if the default scene is empty, go to the *Scenes* folder and double-click on *MyScene.unity* in order to load the project's scene.
- If an environment is added to the project, it should be added as a child GameObject to the *Environments* parent GameObject.
- If a human is added to the project, it should be added as a child GameObject to the *Humans* parent GameObject.
- If a new *AnimationControler* is created, it should be added in the *Assets/Resources/MyAnimatorControllers* folder.
