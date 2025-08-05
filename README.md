🧩 Unity Dynamics Extractor
This Unity Editor tool automates the extraction and organization of VRC PhysBones and VRC PhysBoneColliders from an avatar's hierarchy into a central Dynamics object for easier management and optimization.

✨ Features
📦 Automatically creates a Dynamics GameObject under the avatar root.
Includes physbones and colliders as child containers.

📂 Example Hierarchy
AvatarRoot (with VRCAvatarDescriptor)

└── Dynamics

    ├── physbones
    
    │   └── Hair_PhysBone
    
    └── colliders
    
        └── Shoulder_Collider
        
🛠 How to Use
Place the script inside an Editor folder in your Unity project.
Open your VRChat avatar scene.
Select the top menu: Tools > Extract PhysBones and Colliders.
Done! Your PhysBones and Colliders are now neatly extracted and centralized.

🧩 Requirements
Unity with VRChat SDK3 - Avatars imported. (at date of August 2025)
Works with avatars using VRCPhysBone and VRCPhysBoneCollider.
