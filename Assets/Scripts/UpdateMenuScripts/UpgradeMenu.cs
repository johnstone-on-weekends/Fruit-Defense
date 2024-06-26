
using UnityEngine;

namespace UpdateMenuScripts
{
    public class UpgradeMenu : MonoBehaviour
    {

        public static UpgradeMenu Main;

        public GameObject UpgradeMenuTower { get; private set; }

        [SerializeField] private GameObject upgradeMenuHitbox;
        [SerializeField] private SpriteRenderer selectedSpriteRenderer;
        [SerializeField] private Transform upgradeMenuLeftPosition;
        [SerializeField] private Transform upgradeMenuRightPosition;
        
        
        private LayerMask _towerLayerMask;
        private LayerMask _upgradeMenuLayerMask;

        private bool _towerPlacedThisFrame;
        private Camera _camera;
        private Selector _selectedSpriteSelector;

        private void Awake()
        {
            Main = this;

            _towerLayerMask = LayerMask.GetMask("Tower");
            _upgradeMenuLayerMask = LayerMask.GetMask("Upgrade Menu");
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        private void LateUpdate()
        {
            CheckUpgradeMenuVisibility();
            _towerPlacedThisFrame = false;
        }

        private void CheckUpgradeMenuVisibility()
        {
            // The following will ignore if A: the player hasn't clicked the screen, or B: the player has placed a tower this frame. We don't want to select on such cases.
            if (!Input.GetMouseButtonDown(0) || _towerPlacedThisFrame) return;
            
            Vector2 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

            if (Physics2D.OverlapPoint(mousePosition, _upgradeMenuLayerMask)) return;
        
            Collider2D clickedTower = Physics2D.OverlapPoint(mousePosition, _towerLayerMask);
            

            if (clickedTower)
            {
                if (UpgradeMenuTower && clickedTower.gameObject.GetInstanceID() == UpgradeMenuTower.GetInstanceID()) {
                    return; // Clicked on the tower that's already selected. We can safely ignore
                }
                
                // We've clicked on a tower
                DisplayUpgradeMenu(clickedTower.gameObject, mousePosition);
                
            }
            else if (_selectedSpriteSelector)
            {
                // We've clicked on something other than a tower or the upgrade menu. This means we hide the upgrade menu.
                HideUpgradeMenu();
            }
        }
        
        private void DisplayUpgradeMenu(GameObject tower, Vector2 mousePosition)
        {
            _selectedSpriteSelector?.DeselectSprite(); // Deselect old sprite
            _selectedSpriteSelector = tower.GetComponent<Selector>(); // Identify new sprite
            _selectedSpriteSelector.SelectSprite(); // Select new sprite
            
            UpgradeMenuTower = tower;
            if (upgradeMenuHitbox.activeSelf)
                selectedSpriteRenderer.sprite = tower.GetComponent<SpriteRenderer>().sprite;
            else
                upgradeMenuHitbox.SetActive(true);

            upgradeMenuHitbox.transform.position = mousePosition.x < 2.7 ? upgradeMenuRightPosition.position : upgradeMenuLeftPosition.position;
            DamageDisplay.UpdateRelevantTowerStats();
            LeftUpgradeIcon.SetUpgradeLeftIcon();
            RightUpgradeIcon.SetUpgradeRightIcon();
        }

        private void HideUpgradeMenu()
        {
            _selectedSpriteSelector?.DeselectSprite(); 
            
            upgradeMenuHitbox.SetActive(false);
            _selectedSpriteSelector = null;
            UpgradeMenuTower = null;
        }

        

        public void TowerWasPlacedThisFramed()
        {
            _towerPlacedThisFrame = true;
        }
    }
}
