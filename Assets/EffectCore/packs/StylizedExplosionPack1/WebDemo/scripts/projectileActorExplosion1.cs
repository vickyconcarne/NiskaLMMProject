using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
public class projectileActorExplosion1 : MonoBehaviour {

    public Transform spawnLocator;
    public float coolDown = 1f;
    private float timer;
    private bool canDropMine;
    public int maxBombs = 2;
    public int currentBombs;

    [Header("Animations")]
    public Animator niskaAnimator;
    public Animator bombUIAnimator;
    public Image bombButtonSprite;
    public Color onColor;
    public Color offColor;
    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;
    public AudioSource audioPlayer;
    [Header("Debug")]
    public TextMeshProUGUI bombText;
    public bool debug;

    [System.Serializable]
    public class projectile
    {
        public string name;
        public Rigidbody bombPrefab;
    }
    public projectile[] bombList;

    string FauxName;

    public int bombType = 0;

    public ParticleSystem muzzleflare;

    public float min, max;

    public bool swarmMissileLauncher = false;
    int projectileSimFire = 1;


    public bool Torque = false;
    public float Tor_min, Tor_max;

    public bool MinorRotate;
    public bool MajorRotate = false;
    int seq = 0;


	// Use this for initialization
	void Start ()
    {
        //UiText.text = bombList[bombType].name.ToString();
        if (swarmMissileLauncher)
        {
            projectileSimFire = 5;
        }
        if (debug)
        {
            bombText.text = currentBombs.ToString();
        }
        //pickupSound = Resources.Load("Sounds/MINE_PICKUP") as AudioClip;
    }

    public void AddBomb()
    {
        ActivateBombButton();
        currentBombs += 1;
        if(currentBombs >= maxBombs) {
            currentBombs = maxBombs;
        }
        if (debug)
        {
            bombText.text = currentBombs.ToString();
        }
        audioPlayer.PlayOneShot(pickupSound, 0.5f);
    }

    public void ActivateBombButton()
    {
        bombUIAnimator.SetTrigger("Pop");
        bombButtonSprite.color = onColor;
    }

    public void RemoveBomb()
    {
        currentBombs -= 1;
        canDropMine = false;
        bombButtonSprite.color = offColor;
        if (debug)
        {
            bombText.text = currentBombs.ToString();
        }
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        
        if (timer >= coolDown && !canDropMine)
        {
            canDropMine = true;
            if(currentBombs >0) ActivateBombButton();
        }
        else
        {
            timer += Time.fixedDeltaTime;
        }
    }

    public void Switch(int value)
    {
        bombType += value;
        if (bombType <= 0)
        {
            bombType = bombList.Length + -1;
        }
        if (bombType >= bombList.Length)
        {
            bombType = 0;
        }

        //UiText.text = bombList[bombType].name.ToString();

    }

    public void Fire()
    {
        if (currentBombs <= 0 || !canDropMine) 
        {
            return;
        }
        else
        {
            RemoveBomb();
            bombUIAnimator.SetTrigger("Pop");
            niskaAnimator.SetTrigger("Throw");
        }
        muzzleflare.Play();

        Rigidbody rocketInstance;
        rocketInstance = Instantiate(bombList[bombType].bombPrefab, spawnLocator.position, Quaternion.identity) as Rigidbody;
        rocketInstance.AddForce(spawnLocator.forward * Random.Range(min, max));
        //Reset cooldown
        canDropMine = false;
        timer = 0f;

        if (Torque)
        {
            rocketInstance.AddTorque(spawnLocator.up * Random.Range(Tor_min, Tor_max));
        }
        if (MinorRotate)
        {
            RandomizeRotation();
        }
        if (MajorRotate)
        {
            Major_RandomizeRotation();
        }
    }


    void RandomizeRotation()
    {
        if (seq == 0)
        {
            seq++;
            transform.Rotate(0, 1, 0);
        }
      else if (seq == 1)
        {
            seq++;
            transform.Rotate(1, 1, 0);
        }
      else if (seq == 2)
        {
            seq++;
            transform.Rotate(1, -3, 0);
        }
      else if (seq == 3)
        {
            seq++;
            transform.Rotate(-2, 1, 0);
        }
       else if (seq == 4)
        {
            seq++;
            transform.Rotate(1, 1, 1);
        }
       else if (seq == 5)
        {
            seq = 0;
            transform.Rotate(-1, -1, -1);
        }
    }

    void Major_RandomizeRotation()
    {
        if (seq == 0)
        {
            seq++;
            transform.Rotate(0, 25, 0);
        }
        else if (seq == 1)
        {
            seq++;
            transform.Rotate(0, -50, 0);
        }
        else if (seq == 2)
        {
            seq++;
            transform.Rotate(0, 25, 0);
        }
        else if (seq == 3)
        {
            seq++;
            transform.Rotate(25, 0, 0);
        }
        else if (seq == 4)
        {
            seq++;
            transform.Rotate(-50, 0, 0);
        }
        else if (seq == 5)
        {
            seq = 0;
            transform.Rotate(25, 0, 0);
        }
    }
}
