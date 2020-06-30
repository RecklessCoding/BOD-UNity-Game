# What is BOD-UNity-Game (BUNG)?
BOD-UNity-Game (BUNG) is purpose-made serious game. It is a team-based Capture the Flag (CTF) developed in the popular games engine Unity, where the `player' develop---or tune existing---agents. BUNG is designed to be used as an educational platform to teach developers, such as students undertaking AI courses, to understand how to build complete complex agents by using the Behaviour Oriented Design cognitive architecture. BUNG is designed to allow a variety of strategies at both individual agent and team levels. The game comes as an uncompiled Unity project, with sample agents and an integrated process for fast prototyping BOD agents. The game is being used, since 2017 in the course Intelligent Cognitive Control Systmse at the University of Bath. 

# Instructions
## Downloading the Project
1. Press 'Clone or download' in the upper right

## Getting Started
1. Open Assets/AgentsSubmission folder.
2. Duplicate (and rename) the StudentBlTemplate.class file (Ctrl+D) or create a new class that inherits from the BehaviourLibraryLinker class.
3. Create a new POSH XML plan file. See the two sample plans included in the Resources.
4. Duplicate either the RunAndShoot.prefub or the RunToBase.prefub file.
5. Add your new behaviour library and plan files to the prefub created on the step above. This will be an agent.

## Adding Your bot
1. Open Assets/AgentsSubmission folder.
2. Duplicate (and rename) the SampleTeam.prefub file. This prefub will be your team.
3. Drag & drop the prefub of an agent to one or more elements of the Team Members array, located on the TeamMembers component of your team's prefub. Make sure all array elements are populated.
4. Drag & drop your team to either variable of the TeamSubmissions component of the SubmissionsHere gameobject. Make sure there is a team allocated at each of the variables.
5. Press play!!!

## Upload Your Submission
You need to submit ALL the code and prefubs you are using. If you are using any additional libraries, make sure you submit them two. We recommend against using any proprietary, configuration-specific code.

# Credits for assets and code used:
- Samuel Arminana - Original game design and assets integration
- Denis Sylkin - RTS Camera
- Unity - Unity Particle Pack
- Marcelo Fernanadez Music - Music
- Dogzerx (Jose Diaz) - Cartoon Soldier
- Telecaster - Cloth animation based Flag 1.0
