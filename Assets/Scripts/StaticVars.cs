using System.Collections.Generic;
using UnityEngine;

public static class StaticVars {

    public static List<Sprite> enemySprites = new List<Sprite>(), itemSprites = new List<Sprite>(), shotSprites = new List<Sprite>(), bossSprites = new List<Sprite>();
    public static Game game;
    public static Player player;
    public static GameObject canvas, enemyExp, playerExp, bossExp, enemyShotExp, enemyShotDie, bossHealthBar, wormHole, laser, shot;
    public static Minimap miniMap;
    public static List<AudioClip> shotSounds = new List<AudioClip>();
    public static AudioSource enemyExpSnd, bossExpSnd;
    public static int maxShotAge = 100;
    public static string[] itemNames = {"a twin gun",
                                        "a triple gun",
                                        "a sextuple gun",
                                        "a plasma gun",
                                        "a piercing gun",
                                        "an armor plating",
                                        "a plasma engine",
                                        "a repair kit",
                                        "a laser gun",
                                        "a healing gun",
                                        "an ice gun",
                                        "a lucky horseshoe",
                                        "a spiked hull",
                                        "a snail gun",
                                        "an unstable generator",
                                        "a homing-shot gun",
                                        "a turret",
                                        "a damage booster",
                                        "a firerate booster",
                                        "a hull strength booster",
                                        "a repair booster",
                                        "a speed booster",
                                        "a shot speed booster",
                                        "a life steal booster",
                                        "a damage reflection booster",
                                        "a critical chance booster",
                                        "a critical damage booster",
                                        "a stunning time booster",
                                        "a slowing time booster",
                                        };
}
