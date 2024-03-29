/**
Créer une classe abstraite Pokemon avec une fonction abstraite Crier(). 
Les pokemons ont aussi une proriété (get, set) "Speed" de type int.
Définir deux espèces de pokemons Salameche et Carapuce et définir la fonction Crier() de chacun.

Créer la classe “PokemonTrainer” qui a comme propriétés:
- Name
- Une liste de pokemons (utiliser la classe List)

Créer deux "PokemonTrainer" en leur attribuant un pokemon différent de l'espèce Salameche et un Capauce
*/
abstract class Pokemon
{
    // protected: comme un public dans les sous classes, comme du private en dehors
    public abstract void Crier(); // doit être implémentée par la sous-classe, donc elle ne peut être private
    public int Speed { get; set; }
    public int HealthPoints { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }

    public override string ToString() => $"{GetType().Name}, speed: {Speed}";
}

class Carapuce : Pokemon, IWalker, ISwimmer
{
    // Je mets override pour signifier que j'implémente la méthode virtuelle
    public override void Crier()
    {
        Console.WriteLine("Carapuuuuuuuuuce");
    }

    public void Walk()
    {
        Console.WriteLine("tok tok");
    }

    public void Swim()
    {
        Console.WriteLine("flou flou");
    }
}

class Salameche : Pokemon, IFlyer, IWalker
{
    public override void Crier()
    {
        Console.WriteLine("Sale la mèche, lave toi les cheveux");
    }

    public void Walk()
    {
        Console.WriteLine("tic toc");
    }

    public void Fly()
    {
        Console.WriteLine("vou vou zella");
    }
}
Carapuce carapuce = new Carapuce() { Attack = 5, Defense = 2, HealthPoints = 30 };
carapuce.Speed = 10;

carapuce.Crier();
Salameche salameche = new Salameche() { Speed = 4, Attack = 4, Defense = 4, HealthPoints = 40 };
salameche.Crier();

class PokemonTrainer
{
    public PokemonTrainer(string name, List<Pokemon> pokemons)
    {
        this.Name = name;
        this.Pokemons = pokemons;
    }
    public string Name { get; set; }
    public List<Pokemon> Pokemons { get; set; }
}

// tableau dont on peut ajouter ou supprimer des éléments
var pokemons = new List<Pokemon> { carapuce };
pokemons.Add(salameche);
pokemons.Add(new Carapuce() { Speed = 20 });
PokemonTrainer sacha = new PokemonTrainer("Sacha", pokemons);
Console.WriteLine(String.Join(" - ", sacha.Pokemons));
var pokemons2 = new List<Pokemon> { new Carapuce() { Speed = 5 }, new Salameche() { Speed = 6 } };
PokemonTrainer ondine = new PokemonTrainer("Ondine", pokemons2);

/**
Créer les interfaces suivantes:
    - IWalker avec la fonction Walk
    - IFlyer avec la fonction Fly
    - ISwimmer ave la fonction Swim

Implémenter les interfaces Walker et Swimmer pour Carapuce
Implémenter les interfaces Walker et Flyer pour Salameche

Créer un fonction qui prend un liste de pokemons en paramètre et retourne les pokemons 
qui peuvent nager.
*/
interface IWalker
{
    void Walk();
}
interface IFlyer
{
    void Fly();
}
interface ISwimmer
{
    void Swim();
}

List<Pokemon> GetSwimmers(List<Pokemon> pokemons)
{
    var swimmers = new List<Pokemon>();
    // procédural
    foreach (var pokemon in pokemons)
    {
        if (pokemon is ISwimmer)
        {
            swimmers.Add(pokemon);
        }
    }
    return swimmers;
}

// LINQ
List<Pokemon> GetSwimmersLinq(List<Pokemon> pokemons)
{
    //Method syntax
    return pokemons.Where(p => p is ISwimmer)
                    .OrderByDescending(p => p.Speed)
                    .ToList();
}

List<Pokemon> GetSwimmersLinq2(List<Pokemon> pokemons)
{
    //Query syntax
    return (from p in pokemons
            where p is ISwimmer
            orderby p.Speed descending
            select p).ToList();
}

var swimmers = GetSwimmers(sacha.Pokemons);
Console.WriteLine(String.Join(", ", swimmers));
Console.WriteLine(String.Join(", ", GetSwimmersLinq(pokemons)));
Console.WriteLine(String.Join(", ", GetSwimmersLinq2(pokemons)));

/**
Créer une classe "GameEngine" qui définit une fonction statique "PredictTurns".

Cette fonction prend deux pokemons en paramètres et un entier 'turns'. Elle retourne en résultat une liste de pokemons.
La liste est de taille 'turns'.

Chaque élément de la liste contient le pokemon qui va agir dans le prochaine tour i

Par exemple: PredictTurns(pokemon1, pokemon2, 4) peut retourner {pokemon1, pokemon2, pokemon1, pokemon1} 
-> Cela singifie que dans le tour suivant, Pokemon1 va agir, ensuite Pokemon2, ensuite Pokemon1 et efin Pokemon1.

Le tableau des prochains tours est construit de la façon suivante:
1- Au début, on associe à chaque pokemon une variable 'action' qui vaut 0.
2- Incrémenter (+1) l'action de chaque pokemon et répéter cela jusqu'à atteindre le "Speed" d'un des deux Pokemon. 
3- Celui dont l'action atteint le "Speed" en premier agit (prend le tour) et remet sa variable action à 0. 
    L'autre pokemon garde son action intacte
4- Si l'action est atteinte pour les deux pokemons à la fois, choisir un des deux au hasard et on remet l'action du pokemon choisi à 0. 
    L'action de l'autre pokemon reste intacte.
5- Pour le tour suivant, reprendre à l'étape 2.

par exemple:
Pokemon A -> vitesse 10, Pokemon B -> vitesse 4
Déroulement des tours: B, B, A, B, 

actionA => 4, ActionB => 4 -> B (actionB => 0)
ActionA => 8, ActionB => 4 -> B (actionB => 0)
ActionA => 10, ActionB => 2 -> A (actionA => 0)

Pokemon A -> vitesse 4, Pokemon B -> vitesse 4
Déroulement possible des tours: A, B, A, B, 

Pokemon A -> vitesse 2, Pokemon B -> vitesse 3
Déroulement (possible) des tours: A, B, A, A, B, A,... ou A, B, A, B, A,
*/

static class GameEngine
{
    public static List<Pokemon> PredictTurns(Pokemon pokemon1, Pokemon pokemon2, int turns)
    {
        Console.WriteLine($"Start predict turns {pokemon1} vs {pokemon2}. Fighto !");
        var predictions = new List<Pokemon>();
        int action1 = 0;
        int action2 = 0;

        var r = new Random();
        while (predictions.Count != turns)
        {
            action1 += 1;
            action2 += 1;

            if (action1 >= pokemon1.Speed && action2 >= pokemon2.Speed)
            {
                if (r.Next(1) == 0)
                {
                    predictions.Add(pokemon1);
                    action1 = 0;
                }
                else
                {
                    predictions.Add(pokemon2);
                    action2 = 0;
                }
            }
            else if (action1 >= pokemon1.Speed)
            {
                predictions.Add(pokemon1);
                action1 = 0;
            }
            else if (action2 >= pokemon2.Speed)
            {
                predictions.Add(pokemon2);
                action2 = 0;
            }
        }
        return predictions;
    }

    public static Pokemon SimulateCombat(Pokemon pokemon1, Pokemon pokemon2)
    {
        var predictions = PredictTurns(pokemon1, pokemon2, 20);

        var random = new Random();
        int currentTurn = 0;

        while (currentTurn < predictions.Count && pokemon1.HealthPoints > 0 && pokemon2.HealthPoints > 0)
        {
            Pokemon attackPokemon = predictions[currentTurn];
            Pokemon defensePokemon = attackPokemon == pokemon1 ? pokemon2 : pokemon1;
            var randomMultiplicator = random.NextDouble() + 0.5; // car 0 <= random.NextDouble() <= 1
            var damage = (int)(attackPokemon.Attack * randomMultiplicator - defensePokemon.Defense);
            if (damage <= 0)
            {
                Console.WriteLine($"{attackPokemon.GetType().Name} n'a pas fait de dégats");
            }
            else
            {
                defensePokemon.HealthPoints = defensePokemon.HealthPoints - damage;
                Console.WriteLine($"{attackPokemon.GetType().Name} attaque");
                Console.WriteLine($"{defensePokemon.GetType().Name} subit {damage} degats");
                Console.WriteLine($"{defensePokemon.GetType().Name} a {defensePokemon.HealthPoints} HP");
            }
            currentTurn += 1;
        }
        var winner = pokemon1.HealthPoints > pokemon2.HealthPoints ? pokemon1 : pokemon2;
        Console.WriteLine($"And the winner is {winner.GetType().Name}");
        return winner;
    }
}


var turns = GameEngine.PredictTurns(salameche, carapuce, 10);

Console.WriteLine($"Predicted turns:");
Console.WriteLine(String.Join("\n", turns));

GameEngine.SimulateCombat(salameche, carapuce);

/**
Ajouter les propriétés Attack, Defense et HealthPoints (points de vie) aux pokemons.
Ces valeurs sont initialisées par défaut de cette facon:
- attack: valeur aléatoire entre 5 et 10
- défense: valeur aléatoire entre 5 et 10
- contrainte: attack + défense <= 15
- HealthPoints: valeut aléatoire entre 30 et 40

 Créer dans GameEngine, une fonction statique "SimulateCombat" qui prend deux pokemons en paramètres
et retoune le pokemon qui aura gagné le simulation de combat après 20 tours.

La fonction affiche dans la console le déroulement de la "simulation" de combat.

La simulation se déroule de cette façon:
- Le tour de chaque pokemon est dans l’ordre défini dans la partie précédente.
- Le pokemon qui le moins de pv a perdu
- Dès qu'un pokemon est ko, la simulation s'arrête et ce dernier a perdu

Le pokemon qui subit l’attaque voit ses pv réduire avec cette formule: 
pv_attaqué = pv_attaqué - ( attaque_de_l’attaquant * (multiplicateur alétoire en 0.5 et 1.5) -  défense_attaqué)

Exemple:
Pokemon A -> vitesse 10, attaque 5, défense 2, hp 30
Pokemon B -> vitesse 4, attaque 4, défense 3, hp 40
Déroulement: Pokemon A attaque, pokemon B subit 2 dégats. Pokemon B a 38 hp
*/
