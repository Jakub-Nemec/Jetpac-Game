using System.Collections;
using UnityEngine;

public class Locomotion : MonoBehaviour
{
    private static readonly float DROP_ZONE_X = 44f;  // Konstanta pro určení x-ové pozice zóny, kam se objekt může dostat

    private Rigidbody2D _rb;  // Komponenta Rigidbody2D pro pohyb objektu

    private bool _fired;  // Příznak, zda byla stisknuta klávesa pro střelbu
    private float _maxXSpeed = 32f;  // Maximální horizontální rychlost objektu na zemi
    private float _maxXAirSpeed = 64f;  // Maximální horizontální rychlost objektu ve vzduchu
    private float _maxYSpeed = 64f;  // Maximální vertikální rychlost objektu
    private float _verticalMultiplier = 16f;  // Multiplikátor pro vertikální pohyb objektu
    private float _horizontalMuliplier = 16f;  // Multiplikátor pro horizontální pohyb objektu

    private int _nextPartId = 1;  // ID další součásti
    private GameObject _currentPart;  // Aktuální součást
    private GameObject _currentFuelCell;  // Aktuální palivový článek

    public GameObject cloud;  // Prefab pro vytvoření mraku
    public GameObject shape;  // Prefab pro tvar objektu
    public JetmanAnimation jetmanShape;  // Komponenta pro animaci objektu
    public JetmanShadow shadow;  // Stín objektu

    public GameObject bulletPrefab;  // Prefab pro vytvoření střely

    public Transform bulletSpawnPoint;  // Pozice, ze které se vytvářejí střely

    private Vector2 _velocity;  // Rychlost objektu
    private float _gravity = 0f;  // Hodnota gravitace
    private bool _applyGravity = false;  // Příznak, zda se aplikuje gravitace

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();  // Získání komponenty Rigidbody2D
        shape.SetActive(false);  // Deaktivace tvaru objektu
    }

    public void ResetPartRequired()
    {
        _nextPartId = 1;  // Resetování ID další součásti
    }

    IEnumerator Start()
    {
        // Zajištění, že Jetman padá, pokud je jen mírně nad zemí
        _applyGravity = true;

        while (RocketManager.Instance.State == RocketState.Landing)
        {
            yield return null;  // Čekání na další snímek
        }

        float bulletCooldown = Bullet.COOLDOWN_PERIOD;  // Prodleva mezi výstřely
        AlienManager alienManager = FindObjectOfType<AlienManager>();  // Získání instance správce nepřátel

        while (true)
        {
            if (_fired)
            {
                _fired = false;  // Zrušení stisku klávesy pro střelbu
                var starting = bulletSpawnPoint.localPosition;  // Výchozí pozice střely
                if (jetmanShape.FlipHorizontal)  // Pokud je objekt převrácen horizontálně
                {
                    starting *= -1;  // Inverze pozice
                }
                starting += jetmanShape.transform.position;  // Posunutí pozice na správné místo

                Color color = alienManager.spectrumColours[Random.Range(0, alienManager.spectrumColours.Length)];  // Náhodná barva pro střelu

                for (int i = 0; i < 16 + Random.Range(0, 4); i++)  // Vytvoření více střel
                {
                    var bullet = Instantiate(bulletPrefab);  // Vytvoření střely
                    bullet.GetComponent<SpriteRenderer>().color = color;  // Nastavení barvy střely
                    bullet.transform.position = starting;  // Nastavení pozice střely

                    float direction = jetmanShape.FlipHorizontal ? -8f : 8f;  // Směr střely
                    starting += new Vector3(direction, 0, 0);  // Posunutí výchozí pozice střely
                    if (starting.x < -128f)  // Přetečení zóny na levé straně
                    {
                        starting.x += 256f;  // Posunutí na pravou stranu
                    }
                    else if (starting.x > 128f)  // Přetečení zóny na pravé straně
                    {
                        starting.x -= 256f;  // Posunutí na levou stranu
                    }
                }
                yield return new WaitForSeconds(bulletCooldown);  // Prodleva mezi střelami
            }
            else
            {
                yield return null;  // Čekání na další snímek
            }
        }
    }

    private void Update()
    {
        if (RocketManager.Instance.State == RocketState.Landing)  // Kontrola stavu raketoplánu
        {
            return;  // Pokud raketoplán přistává, přerušení metody
        }

        if (!shape.activeInHierarchy)
        {
            shape.SetActive(true);  // Aktivace tvaru objektu
        }

        var horiz = Input.GetAxis("Horizontal");  // Získání hodnoty horizontálního vstupu
        var vert = Input.GetAxis("Vertical");  // Získání hodnoty vertikálního vstupu

        _fired = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);  // Zjištění, zda byla stisknuta klávesa pro střelbu

        vert = vert > 0 ? vert : 0f;  // Zápornou vertikální hodnotu převádíme na nulu

        if (vert != 0)
            _gravity = 0f;  // Resetování hodnoty gravitace, pokud je aktivní vertikální pohyb

        if (horiz == 0 && vert == 0)  // Pokud se objekt nepohybuje
        {
            jetmanShape.Idle();  // Spuštění animace nečinnosti
        }
        else
        {
            jetmanShape.FlipHorizontal = horiz < 0;  // Převrácení objektu horizontálně
            jetmanShape._isAnimating = true;  // Spuštění animace
        }

        _velocity += new Vector2(horiz * _horizontalMuliplier, vert * _verticalMultiplier);  // Výpočet nové rychlosti objektu

        var maxXSpeed = jetmanShape._isInflight ? _maxXAirSpeed : _maxXSpeed;  // Určení maximální horizontální rychlosti podle stavu objektu (vzduch/nevzduch)
        if (Mathf.Abs(_velocity.x) > maxXSpeed)
            _velocity.x = maxXSpeed * Mathf.Sign(_velocity.x);  // Omezení horizontální rychlosti na maximální hodnotu

        if (Mathf.Abs(_velocity.y) > _maxYSpeed)
            _velocity.y = _maxYSpeed * Mathf.Sign(_velocity.y);  // Omezení vertikální rychlosti na maximální hodnotu

        if (horiz == 0f)
            _velocity.x = 0f;  // Resetování horizontální rychlosti, pokud není aktivní horizontální pohyb

        if (vert == 0f)
            _velocity.y = 0f;  // Resetování vertikální rychlosti, pokud není aktivní vertikální pohyb

        if (_velocity.y == 0 && _applyGravity)  // Pokud je objekt v klidu ve vzduchu a aplikuje se gravitace
            _gravity += -_maxXAirSpeed * Time.deltaTime;  // Aplikace gravitace

        _velocity.y += _gravity;  // Přidání gravitace k vertikální rychlosti
        _rb.velocity = _velocity;  // Nastavení nové rychlosti objektu
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Alien")  // Pokud došlo ke kolizi s objektem s tagem "Alien"
        {
            GameManager.Instance.PlayerDied();  // Zavolání metody pro smrt hráče
        }
        else if (collision.gameObject.tag == "Platform")  // Pokud došlo ke kolizi s objektem s tagem "Platform"
        {
            if (gameObject.transform.position.y > collision.gameObject.transform.position.y)  // Pokud je objekt nad platformou
            {
                jetmanShape.Idle();  // Spuštění animace nečinnosti
                jetmanShape._isInflight = false;  // Přepnutí příznaku letu objektu
                _applyGravity = false;  // Vypnutí aplikace gravitace
                _gravity = 0;  // Resetování hodnoty gravitace
            }
        }
    }
    // Metoda volaná při opuštění kolize objektu
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Kontrola, zda se kolize stala s objektem s tagem "Platform"
        if (collision.gameObject.tag == "Platform")
        {
            // Vytvoření kopie objektu "cloud" (mlhy)
            var copy = Instantiate(cloud);

            // Nastavení pozice kopie na pozici aktuálního objektu s posunem o 8 jednotek dolů
            copy.transform.position = gameObject.transform.position - new Vector3(0, 8, 0);

            // Nastavení příznaku letu postavy
            jetmanShape._isInflight = true;

            // Povolení gravitace
            _applyGravity = true;
        }
    }

    // Metoda volaná při vstupu do trigger kolize s objektem
    public void OnTriggerEnter2D(Collider2D collision)
    {
        // Kontrola, zda se kolize stala s objektem s tagem "RocketPart"
        if (collision.tag == "RocketPart")
        {
            // Předání objektu "RocketPart" metode "PickupRocketPart" pro zvednutí
            PickupRocketPart(collision.gameObject);
        }
        // Kontrola, zda se kolize stala s objektem s tagem "Pickup"
        else if (collision.tag == "Pickup")
        {
            // Získání kořenového objektu kolize
            var root = collision.gameObject.transform.root.gameObject;

            // Získání komponenty "Pickup" ze kořenového objektu
            var pickup = root.GetComponent<Pickup>();

            // Nastavení příznaku "pickedUp" na hodnotu true
            pickup._pickedUp = true;

            // Předání kořenového objektu metodě "PickupObject" třídy "DropManager" pro zvednutí objektu
            DropManager.Instance.PickupObject(root);

            // Zvýšení skóre za zvednutí drahokamu
            ScoreManager.Instance.PickUpGem();
        }
        // Kontrola, zda se kolize stala s objektem s tagem "Fuel"
        else if (collision.tag == "Fuel")
        {
            // Získání kořenového objektu kolize
            var root = collision.gameObject.transform.root.gameObject;

            // Kontrola, zda objekt "Pickup" na kořenovém objektu neklesá
            if (!root.GetComponent<Pickup>()._falling)
            {
                // Předání kořenového objektu metodě "PickupFuel" pro zvednutí paliva
                PickupFuel(root);

                // Zvýšení skóre za zvednutí paliva
                ScoreManager.Instance.PickUpFuel();
            }
        }
        // Kontrola, zda se kolize stala s objektem s tagem "LeftSideBlock"
        else if (collision.tag == "LeftSideBlock")
        {
            // Nastavení hráče v "shadow" na aktuálního hráče s posunem o 256 jednotek doprava
            shadow.SetPlayer(this, new Vector3(256, 0));
        }
        // Kontrola, zda se kolize stala s objektem s tagem "RightSideBlock"
        else if (collision.tag == "RightSideBlock")
        {
            // Nastavení hráče v "shadow" na aktuálního hráče s posunem o 256 jednotek doleva
            shadow.SetPlayer(this, new Vector3(-256, 0));
        }
    }

    // Metoda volaná při opuštění trigger kolize s objektem
    public void OnTriggerExit2D(Collider2D collision)
    {
        // Kontrola, zda se kolize stala s objektem s tagem "RightSideBlock"
        if (collision.tag == "RightSideBlock")
        {
            // Kontrola, zda se aktuální pozice hráče nachází vpravo od pozice objektu kolize
            if (transform.position.x > collision.gameObject.transform.position.x)
            {
                // Získání světové pozice "shadow"
                var worldPosition = shadow.transform.position;

                // Nastavení pozice hráče na světovou pozici "shadow"
                transform.position = worldPosition;

                // Zrušení hráče v "shadow" (nastavení na null) a resetování jeho pozice na nulový vektor
                shadow.SetPlayer(null, Vector3.zero);
            }
            else
            {
                // Zrušení hráče v "shadow" (nastavení na null) a resetování jeho pozice na nulový vektor
                shadow.SetPlayer(null, Vector3.zero);
            }
        }
        // Kontrola, zda se kolize stala s objektem s tagem "LeftSideBlock"
        else if (collision.tag == "LeftSideBlock")
        {
            // Kontrola, zda se aktuální pozice hráče nachází vlevo od pozice objektu kolize
            if (transform.position.x < collision.gameObject.transform.position.x)
            {
                // Získání světové pozice "shadow"
                var worldPosition = shadow.transform.position;

                // Nastavení pozice hráče na světovou pozici "shadow"
                transform.position = worldPosition;

                // Zrušení hráče v "shadow" (nastavení na null) a resetování jeho pozice na nulový vektor
                shadow.SetPlayer(null, Vector3.zero);
            }
            else
            {
                // Zrušení hráče v "shadow" (nastavení na null) a resetování jeho pozice na nulový vektor
                shadow.SetPlayer(null, Vector3.zero);
            }
        }
    }

    // Metoda volaná při pobytu v trigger kolizi s objektem
    public void OnTriggerStay2D(Collider2D collision)
    {
        // Kontrola, zda se kolize stala s objektem s názvem "DropZone" 
        // a zároveň zda je vzdálenost mezi pozicí hráče a pozicí objektu kolize menší než 2
        if (collision.name == "DropZone" && Mathf.Abs(transform.position.x - collision.transform.position.x) < 2)
        {
            // Kontrola, zda je aktuální součástí není null
            if (_currentPart != null)
            {
                // Předání aktuální součásti metodě "DropItem" pro odhození
                DropItem(_currentPart);

                // Předání aktuální součásti třídě "RocketManager" pro odhození součásti rakety
                RocketManager.Instance.DropPart(_currentPart, _currentPart.GetComponent<RocketPart>().partId);

                // Nastavení aktuální součásti na null
                _currentPart = null;

                // Snížení hodnoty id následující součásti
                _nextPartId--;
            }
            // Kontrola, zda je aktuální palivovou buňkou není null
            else if (_currentFuelCell != null)
            {
                // Předání aktuální palivové buňky metodě "DropItem" pro odhození
                DropItem(_currentFuelCell);

                // Nastavení tagu palivové buňky na "Dropped Fuel"
                _currentFuelCell.GetComponent<BoxCollider2D>().tag = "Dropped Fuel";

                // Povolení kolize palivové buňky
                _currentFuelCell.GetComponent<BoxCollider2D>().enabled = true;

                // Povolení fyziky palivové buňky
                _currentFuelCell.GetComponent<Rigidbody2D>().isKinematic = false;

                // Nastavení příznaku "falling" na hodnotu true pro palivovou buňku
                _currentFuelCell.GetComponent<Pickup>()._falling = true;

                // Nastavení aktuální palivové buňky na null
                _currentFuelCell = null;
            }
        }
    }

    // Metoda pro odhození objektu
    private void DropItem(GameObject go)
    {
        // Zrušení rodičovského objektu
        go.transform.SetParent(null);

        // Nastavení pozice objektu na novou pozici s pevnou X-ovou souřadnicí
        go.transform.position = new Vector3(DROP_ZONE_X, go.transform.position.y);
    }

    // Metoda pro zvednutí součásti rakety
    private void PickupRocketPart(GameObject rocketPart)
    {
        // Kontrola, zda id součásti rakety odpovídá očekávanému id
        if (rocketPart.GetComponent<RocketPart>().partId != _nextPartId) return;

        // Nastavení rodičovského objektu součásti rakety na aktuálního hráče
        rocketPart.transform.SetParent(transform);

        // Nastavení lokální pozice součásti rakety na nulový vektor
        rocketPart.transform.localPosition = Vector3.zero;

        // Zakázání kolize součásti rakety
        rocketPart.GetComponent<BoxCollider2D>().enabled = false;

        // Nastavení aktuální součásti na zvednutou součást
        _currentPart = rocketPart;
    }

    // Metoda pro zvednutí palivové buňky
    private void PickupFuel(GameObject fuel)
    {
        // Kontrola, zda aktuální palivová buňka není null
        if (_currentFuelCell != null) return;

        // Nastavení rodičovského objektu palivové buňky na aktuálního hráče
        fuel.transform.SetParent(transform);

        // Nastavení lokální pozice palivové buňky na nulový vektor
        fuel.transform.localPosition = Vector3.zero;

        // Zakázání kolize palivové buňky
        fuel.GetComponent<BoxCollider2D>().enabled = false;

        // Zakázání fyziky palivové buňky
        fuel.GetComponent<Rigidbody2D>().isKinematic = true;

        // Nastavení aktuální palivové buňky na zvednutou buňku
        _currentFuelCell = fuel;
    }
}

