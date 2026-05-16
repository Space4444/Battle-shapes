using System.Collections.Generic;
using UnityEngine;

public class SetVars : MonoBehaviour {

    public List<Sprite> enemySprites = new List<Sprite>(), itemSprites = new List<Sprite>(), shotSprites = new List<Sprite>(), bossSprites = new List<Sprite>();
    public Game game;
    public Player player;
    public GameObject canvas, enemyExp, playerExp, bossExp, enemyShotExp, enemyShotDie, bossHealthBar, wormHole, laser, shot;
    public Minimap miniMap;
    public List<AudioClip> shotSounds = new List<AudioClip>();
    public AudioSource enemyExpSnd, bossExpSnd;

    void Awake () {
        StaticVars.enemySprites = enemySprites;
        StaticVars.shotSprites = shotSprites;
        StaticVars.itemSprites = itemSprites;
        StaticVars.bossSprites = bossSprites;
        StaticVars.game = game;
        StaticVars.player = player;
        StaticVars.canvas = canvas;
        StaticVars.enemyExp = enemyExp;
        StaticVars.playerExp = playerExp;
        StaticVars.bossExp = bossExp;
        StaticVars.enemyShotExp = enemyShotExp;
        StaticVars.enemyShotDie = enemyShotDie;
        StaticVars.bossHealthBar = bossHealthBar;
        StaticVars.miniMap = miniMap;
        StaticVars.wormHole = wormHole;
        StaticVars.shotSounds = shotSounds;
        StaticVars.enemyExpSnd = enemyExpSnd;
        StaticVars.bossExpSnd = bossExpSnd;
        StaticVars.laser = laser;
        StaticVars.shot = shot;
        Destroy(this);
	}
}
