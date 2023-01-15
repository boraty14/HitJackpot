This project is developed according to given case instructions. You can play the game from
game scene. For visual part, you can click spin button to get a random spin animation between
slow, normal and fast. You can set visual spinning settings from "Assets/Game/Data/Spin Settings.asset"
Additionally, you can test a specific spin animation from GameScene hierarchy "UI/SlotModel/SlowWindow"
SlotController script. You can use the buttons to test the given animation, you should enter play mode to
test it.

For game logic and backend, I used a system for predetermining all the spin results beforehand.
There are a couple of reasons for this decision. First, since we want a distribution close to perfect
and distribute each possibility in their given interval equally, with a relatively less randomness the distribution 
results should be somewhat similar. Also by setting results before hand, we avoid doing calculations during playtime,
while the user is spinning and therefore optimizing the game experience. In this case I use a scriptable object to generate
spin result according to given probabilities and store them. From "Assets/Game/Data/Spin Data.asset" you can use generate
spin results button and the results will be generated, stored in "Spin Result List" scriptable object
and printed to console with each results appear indices and other details.
During play, results will be indexed from this result list. If it is not created, it will be when game
starts. For save system, I serialize the spin result list data and save it to a file in persistent data path in json format encrypted,
using a package called EasySave. It lets me to serialize and deserialize data for save and load system. When the game starts,
it first checks if there is a saved data and if there is, it uses this data and continues from the last spinned result using saved
index. The important part is scriptable object is not used for save system by itself. It saves its state in the editor but
in the build this is not the case. This is why we need to serialize and store the data elsewhere and load it.
If there is no save file, it uses the one we created from editor and if we did not, it generates one itself when the game starts.
For the details of the algorithm which creates spin results and distributes each probability in almost perfect intervals, you can see
"Assets/Game/Scripts/GameElements/SpinData.cs". You can change the probability settings from "Assets/Game/Data/Spin Generator.asset"


