# 🎮 Santa Survival

Unity로 개발한 **3D Wave Defense + 로그라이크** 장르의 개인 프로젝트입니다.  

플레이어(산타)는 자동 공격을 기반으로 몬스터를 처치하며 경험치를 획득하고,   
레벨업 시 선택하는 **증강 시스템(Augment System)** 을 통해 캐릭터를 성장시키며 타워를 방어해야 합니다.  

이 프로젝트는 단순 기능 구현이 아닌,

- FSM 기반 상태 관리 구조
- ScriptableObject 기반 데이터 중심 설계
- 증강 선택을 통한 성장 시스템
- 웨이브 기반 난이도 스케일링

을 통해 **확장성과 유지보수를 고려한 게임 시스템 설계**에 초점을 맞춰 개발되었습니다.  

<img width="400" height="225" alt="1_게임시작화면" src="https://github.com/user-attachments/assets/7d3abc07-a2b7-4796-a777-79a874dbe45d" />
<img width="400" height="225" alt="2_컷씬장면" src="https://github.com/user-attachments/assets/e7a309ba-4499-46fe-ae4e-ee12a8775088" />

<img width="400" height="225" alt="4_증강선택" src="https://github.com/user-attachments/assets/bd532072-232e-4590-bd77-9bc89acdf50e" />
<img width="400" height="225" alt="5_게임진행중" src="https://github.com/user-attachments/assets/4671a0fd-607e-402a-864c-c57d7eea1fa1" />

<img width="400" height="225" alt="6_보스몬스터조우" src="https://github.com/user-attachments/assets/a31c04de-0d79-415e-a9e8-8db5da75d130" />
<img width="400" height="225" alt="7_최종보스" src="https://github.com/user-attachments/assets/7349bc39-1135-4681-8871-e6b933a993a0" />

<br>

## 🎮 Genre
3D Wave Defense / Roguelike

## 🎮 Platform
PC (Windows) / Android

<br>

## 🔧 Development Environment

- Engine: Unity 6000.0.54f1 LTS
- Language: C#
- IDE: Microsoft Visual Studio 2022
- Version Control: Git
- OS: Windows 11

<br>

## 📅 Development Period

- **2025.12 ~ 2026.02 (약 2개월)**  
- **Solo Development**

<br>

## 🧱 Project Architecture

이 프로젝트는 다음과 같은 구조를 기반으로 설계되었습니다.

**Architecture**
- FSM (Finite State Machine) 기반 AI 구조
- ScriptableObject 기반 데이터 아키텍처
- Component 기반 게임 시스템 설계

**Core Gameplay Systems**
- Player FSM System
- Monster AI System
- Pet AI System
- Weapon System
- Elemental Combat System
- Level & EXP System
- Augment System
- Wave System
- Tower Defense System

**Supporting Systems**
- Camera Occlusion System
- Async Scene Loading
- Game State Manager
- Cutscene System

<br>

## 🧭 Game Flow

게임은 웨이브 기반으로 진행되며   
몬스터 처치 → 경험치 획득 → 레벨업 → 증강 선택의 루프 구조로 설계되었습니다.  
<img width="450" height="650" alt="game_flow_diagram" src="https://github.com/user-attachments/assets/12289c79-cb71-4310-b0bd-a9a9ad91729d" />

몬스터 웨이브가 시작되면 몬스터가 스폰되고   
플레이어 / 펫 / 타워가 협력하여 몬스터를 처치합니다.  

몬스터 처치 시 경험치를 획득하며   
레벨업 시 **증강(Augment)**을 선택하여 캐릭터를 강화합니다.  

이 루프는 플레이어의 선택(증강, 리롤)에 따라 매 플레이마다 다른 전투 양상이 나오도록 설계되었습니다.

<br>

## 🧠 Core Systems

### FSM 기반 AI 시스템

플레이어, 몬스터, 펫의 상태 처리를 위해 FSM(Finite State Machine) 구조를 사용했습니다.     
공통 StateMachine과 IState 인터페이스를 기반으로 상태를 분리하여 관리합니다.   
상태별 로직을 분리하여 유지보수성과 확장성을 높였습니다.  

* StateMachine
  * ChangeState()
  * Update()

* IState
  * Enter()
  * Execute()
  * Exit()

<br>

### Player FSM

플레이어는 Idle / Move / Dead 상태로 구성됩니다.  

공격 상태를 별도의 FSM으로 두지 않고 자동 공격 시스템을 PlayerController에서 처리하여   
이동 FSM과 전투 로직을 분리했습니다.  

* Idle
  * 이동 입력 발생 → Move

* Move
  * 키보드 이동
  * 마우스 목적지 이동
  * 목적지 도착 → Idle

* Dead
  * 사망 애니메이션 후 제거

<br>

### Monster FSM

몬스터는 Idle / Chase / Attack / Stun / Dead 상태로 구성된 AI 구조를 사용합니다.  

* Idle
  * 타겟 감지 → Chase

* Chase
  * NavMeshAgent 추적
  * 공격 범위 진입 → Attack

* Attack
  * 공격 실행
  * 애니메이션 이벤트 → Chase
  * 일부 몬스터는 투사체 기반 공격을 사용

* Stun
  * 일정 시간 행동 불가

* Dead
  * 사망 처리
  * 애니메이션이 없는 몬스터는 y스케일을 줄이는 연출로 구현

**특징**
- NavMesh 기반 추적 AI
- 애니메이션 이벤트 기반 공격 처리
- 증강 시스템과 연동되는 Stun 상태

<br>

### Pet FSM

펫은 플레이어를 보조하는 서브 타워형 AI 유닛입니다.  

* Idle
  * 타겟 없음 → 대기
  * 타워 반경 이탈 → 복귀
  * 공격 가능 → Attack
  * 타겟 존재 → Chase

* Chase
  * 몬스터 추적
  * 공격 범위 진입 → Attack

* Attack
  * 돌진 공격
  * 공격 후 Idle

* Dead
  * 사망 연출

**특징**
- 타워 반경 기반 활동 범위
- 활동 반경 이탈 시 스폰 위치로 복귀
- 일정 반경 내 몬스터 자동 탐지
- 자동 공격 서브 유닛

<br>

## ⚔️ Weapon System

무기 시스템은 ScriptableObject 기반 데이터 구조로 구현되었습니다.  

* WeaponData (ScriptableObject)
  * WeaponType
  * ElementType
  * Damage
  * AttackRange
  * Prefab
 
무기 데이터는 WeaponDatabase를 통해 관리됩니다.  

무기 종류   
| Weapon    | 특징                                    |
| --------- | --------------------------------------  |
| Sword | 근거리 범위 공격               |
| Rifle | 투사체 공격                |

<br>

## 🔥 Elemental Combat System

게임에는 속성 상성 기반 전투 시스템이 적용되었습니다.  

속성 종류
- Normal
- Fire
- Electric
- Water
- Rock
- Ice

속성 상성에 따라 데미지 배율이 적용됩니다.    
| 관계    | 배율                                    |
| --------- | --------------------------------------  |
| Strong | 1.5x               |
| Weak | 0.5x                |
| Normal | 1.0x                |

속성 배율 계산은 ElementalCombat에서 처리되며   
각 오브젝트에 ElementalStatus 컴포넌트를 통해 적용됩니다.

<br>

## 🧬 Augment System

레벨업 시 증강 카드 3개 중 하나를 선택할 수 있습니다.  

플레이어가 레벨업할 때마다 증강 선택 UI가 표시되며   
선택된 증강은 ScriptableObject 기반 시스템을 통해 Player, Pet, Tower 등에 적용됩니다.  

(증강 적용 흐름)   
<img width="400" height="500" alt="augment_system_diagram" src="https://github.com/user-attachments/assets/e74f138a-da62-4ca9-9b89-52493817e6a5" />

* AugmentData
  * augmentName
  * category
  * effects
  * requiredAugment
  * oneTimeOnly
  * maxStack  


지원하는 증강 효과
- 무기 선택(Sword, Rifle)
- 플레이어 능력치 증가
- 무기 강화
- 무기 속성 변경
- 타워 강화
- 펫 소환
- 펫 능력치 강화
- 몬스터 기절
- 파동탄 공격(패시브 추가)
- 경험치 흡수 범위 증가
- 아군 전체 회복

<br>

## 📈 Level & EXP System

몬스터 처치 시 경험치 Sphere가 드롭됩니다.  

플레이어는 일정 범위 내에서 경험치를 자동으로 흡수합니다.  

경험치가 일정량에 도달하면 플레이어 레벨이 상승하며     
레벨업 시 증강(Augment) 선택 UI가 표시됩니다.

<br>

## 🌊 Wave System

몬스터 처치 → 경험치 획득 → 레벨업 → 증강 선택 → 캐릭터 성장으로 이어지는   
성장 기반 전투 루프를 중심으로 게임이 진행되며,   
몬스터 웨이브를 통해 점진적인 난이도 상승 구조를 설계했습니다.  

각 웨이브마다 몬스터가 등장하며   
게임 진행 상황은 Wave Timer UI로 표시됩니다.
- 현재 웨이브 진행 시간(보스 웨이브일 경우 보스의 체력바로 변경)
- 전체 게임 진행률

* WaveManager
  * Monster Spawn
  * Boss Wave
  * Game Progress UI  

**특징**
- ScriptableObject 기반 WaveData
- 플레이어 레벨 기반 난이도 증가
- 보스 웨이브 시스템

<br>

## 🏰 Tower System

맵 중앙에는 타워 오브젝트가 존재합니다.
- 몬스터는 플레이어와 함께 타워를 공격 대상으로 삼습니다.
- 타워 체력이 0이 되면 게임이 종료됩니다.

타워는 증강 시스템을 통해 "최대 체력 증가", "방어력 증가", "펫 소환" 등의 강화가 가능합니다.  

<br>

## 🎁 Gift Box System

맵 전역에 산타가 잃어버린 선물 상자가 스폰됩니다.  

플레이어는 맵을 탐색하여 선물 상자를 획득할 수 있으며    
획득 시 체력이 회복됩니다.

선물 상자는 웨이브 완료 시 랜덤 위치에 생성되며   
NavMesh 기반 위치 샘플링을 통해 유효한 위치에 스폰됩니다.  

이 시스템은 맵 탐색 요소를 추가하여   
플레이어가 타워 주변에만 머무르지 않도록 설계되었습니다.  

획득한 선물 상자의 개수는 UI를 통해 표시됩니다.  

---

## 🎲 Augment Reroll

획득한 선물 상자는 체력 회복뿐만 아니라   
증강 선택지를 다시 갱신(Reroll)하는 자원으로도 사용됩니다.  

플레이어는 증강 선택 시 선물 상자를 소모하여   
현재 증강 선택지를 새롭게 갱신할 수 있습니다.  

- 회복 vs 리롤 선택 구조
- 플레이 스타일에 따른 전략적 자원 사용
- 로그라이크 랜덤성 제어 수단 제공

<br>

## 🎬 Cutscene System

게임 시작과 클리어 연출을 위해 ScriptableObject 기반 컷씬 시스템을 구현했습니다.  

컷씬 데이터는 이미지와 대사 프레임 구조로 관리됩니다.  

* CutsceneData
  * CutsceneFrame[]
    * Image
    * Dialogues
      
컷씬 진행과 씬 전환은 CutsceneManager가 담당합니다.

<br>

## 🎮 Game State System

GameManager는 게임 전체 상태를 관리합니다.  

게임 상태는 다음과 같이 구성됩니다.  

- Title
- Settings
- Cutscene
- Playing
- Paused
- AugmentSelect
- Result

상태 전환 시 Time.timeScale을 제어하여   
게임 진행과 UI 상태를 관리하도록 구현했습니다.

<br>

## 🌄 Stage System

웨이브 진행에 따라 스테이지 환경이 변경됩니다.   
10웨이브마다 Skybox가 변경되며   
시간대가 낮 → 해질녘 → 밤 → 새벽 → 낮으로 변화합니다.   
마지막 스테이지에서는 눈 효과가 강화되어   
게임 분위기를 연출합니다.
- Skybox
- Lighting
- Ambient

스테이지 변경 이벤트는 StageManager에서 관리되며   
환경 변화는 StageLightingController에서 처리됩니다.

<br>

## ⏳ Async Scene Loading

씬 전환 시 로딩 시간을 개선하기 위해 비동기 씬 로딩을 사용했습니다.  

씬 로딩 중에는 로딩 오버레이 UI를 표시하여   
플레이어가 자연스럽게 게임 시작을 기다릴 수 있도록 구현했습니다.

<br>

## 📂 Project Structure

Scripts
- Player
- Monster
- Pet
- Tower
- Wave
- Augment
- Weapon
- UI
- Managers
- Systems

<br>

## ⭐ Key Features

- FSM 기반 AI 시스템
- ScriptableObject 기반 데이터 설계
- NavMesh 기반 몬스터 AI
- 자동 공격 전투 시스템
- 성장 기반 전투 루프 + 웨이브 난이도 구조
- 속성 상성 전투 시스템
- 레벨 및 경험치 시스템
- 증강 카드 기반 성장 시스템
- 펫 AI 시스템
 
<br>

## 📌 Development Notes

이 프로젝트는 다음 기술 구현을 목표로 개발했습니다.  

- 게임 시스템 설계 능력
- FSM 기반 캐릭터 AI 설계
- ScriptableObject 데이터 구조 설계
- 전투 시스템 설계
- 웨이브 기반 게임 루프 구현
- 증강 기반 성장 시스템 구현

<br>

## 플레이 영상

링크 추가 예정

<br>

## 게임 다운로드

PC:  
모바일:  

<br>

## 기술 문서 (기술서)

프로젝트의 상세 기술 구현 내용은 아래 문서에서 확인할 수 있습니다.   
(링크 추가 예정)

<br>

## 📌 사용 에셋, 애니메이션, 이미지 출처

- Unity Asset Store (무료 에셋)
- Mixamo[https://www.mixamo.com/]
- Flaticon[https://www.flaticon.com/kr/]
