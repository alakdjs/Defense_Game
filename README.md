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

<img width="400" height="225" alt="Title" src="https://github.com/user-attachments/assets/c973f85e-2c64-40d4-b8eb-fca0e5047eaa" />
<img width="400" height="225" alt="cutScene" src="https://github.com/user-attachments/assets/d1256038-16ec-4114-a11c-d558e23121c7" />

<img width="400" height="225" alt="game1" src="https://github.com/user-attachments/assets/ccf76f28-6a96-4084-b021-6e8102cb2be7" />
<img width="400" height="225" alt="game2" src="https://github.com/user-attachments/assets/806a2188-0813-42c4-a7a7-d189b7ade223" />

<img width="400" height="225" alt="game3" src="https://github.com/user-attachments/assets/e0db32ce-ba35-4f12-925d-70b2a95a65dd" />
<img width="400" height="225" alt="game4" src="https://github.com/user-attachments/assets/e344aee5-446d-43d2-8eaf-3348cf53e07d" />

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

- **2025.12.08 ~ 2026.02.02 (약 2개월)**  
- **Solo Development**

<br>

## 🧱 Project Architecture

이 프로젝트는 **데이터 중심 구조 + 상태 기반 로직 분리**를 목표로 설계되었습니다.

### Architecture

- FSM 기반 상태 관리 (Player / Monster / Pet)
- ScriptableObject 기반 데이터 설계
- Component 기반 기능 분리 구조

### Core Gameplay Loop

몬스터 처치 → 경험치 획득 → 레벨업 → 증강 선택 → 전투 강화

이 루프를 중심으로   
웨이브 진행에 따라 난이도가 점진적으로 상승하도록 설계했습니다.

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
- etc.

<br>

## 🧭 Game Flow

게임은 웨이브 기반으로 진행되며   
몬스터 처치 → 경험치 획득 → 레벨업 → 증강 선택의 루프 구조로 설계되었습니다.  
<img width="400" height="750" alt="mermaid-diagram" src="https://github.com/user-attachments/assets/081c3833-7720-4b1b-84e3-b0c1464574f9" />


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

<br>

### Player FSM

플레이어는 Idle / Move / Dead 상태로 구성됩니다.  

공격은 별도의 상태로 분리하지 않고 자동 공격 시스템으로 처리하여   
이동과 전투 로직을 분리했습니다.

<br>

### Monster FSM

몬스터는 Idle / Chase / Attack / Stun / Dead 상태로 구성된 AI 구조를 사용합니다.  

**특징**
- NavMesh 기반 추적 AI
- 애니메이션 이벤트 기반 공격 처리
- 증강 시스템과 연동되는 Stun 상태

<br>

### Pet FSM

펫은 플레이어를 보조하는 서브 타워형 AI 유닛입니다.  
증강으로 펫을 소환할 수 있습니다.  

**특징**
- 타워 반경 기반 활동 범위
- 활동 반경 이탈 시 스폰 위치로 복귀
- 일정 반경 내 몬스터 자동 탐지
- 자동 공격 서브 유닛

<br>

## ⚔️ Weapon System

무기 시스템은 ScriptableObject 기반 데이터 구조로 구현되었습니다.  
 
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

<br>

## 🧬 Augment System

증강 시스템은 **ScriptableObject 기반 데이터 구조**로 설계되었습니다.  

- 레벨업 시 3개의 선택지 제공
- 선택 결과는 Player / Pet / Tower에 즉시 반영
- 선행 조건 및 스택 구조 지원

핵심 특징:

- 데이터 기반 확장 구조 (코드 수정 없이 증강 추가 가능)
- 카테고리 잠금 시스템 (무기 선택 중복 방지)
- 스택 기반 강화 시스템

증강 적용은 AugmentManager에서 통합 관리됩니다.  

(증강 적용 흐름)   
<img width="400" height="500" alt="augment_system_diagram" src="https://github.com/user-attachments/assets/e74f138a-da62-4ca9-9b89-52493817e6a5" />

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

레벨이 오를수록 공격력과 방어력, 체력이 증가하며   
레벨업에 요구되는 경험치 양이 증가합니다.

<br>

## 🌊 Wave System

몬스터 처치 → 경험치 획득 → 레벨업 → 증강 선택 → 캐릭터 성장으로 이어지는   
성장 기반 전투 루프를 중심으로 게임이 진행되며,   
몬스터 웨이브를 통해 점진적인 난이도 상승 구조를 설계했습니다.  

각 웨이브마다 몬스터가 등장하며   
게임 진행 상황은 Wave Timer UI로 표시됩니다.
- 현재 웨이브 진행 시간(보스 웨이브일 경우 보스의 체력바로 변경)
- 전체 게임 진행률

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

선물 상자는 웨이브 완료 시 랜덤 위치에 생성되며   
NavMesh 기반 위치 샘플링을 통해 유효한 위치에 스폰됩니다.  

이 시스템은 맵 탐색 요소를 추가하여   
플레이어가 타워 주변에만 머무르지 않도록 설계되었습니다.  

획득한 선물 상자의 개수는 UI를 통해 표시됩니다.  

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
      
컷씬 진행과 씬 전환은 CutsceneManager가 담당합니다.

<br>

## 🎮 Game State System

GameManager는 게임 전체 상태를 관리합니다.  

게임 상태는 다음과 같이 구성됩니다.  

- Title (타이틀 화면)
- Settings (설정 화면)
- Cutscene (컷씬)
- Playing (게임 중)
- Paused (게임 일시정지)
- AugmentSelect (증강 팝업 창)
- Result (게임 결과, 게임 오버)

상태 전환 시 Time.timeScale을 제어하여   
게임 진행과 UI 상태를 관리하도록 구현했습니다.

<br>

## 🌄 Stage System

웨이브 진행에 따라 스테이지 환경이 변경됩니다.   
10웨이브마다 Skybox가 변경되며   
시간대가 낮 → 해질녘 → 밤 → 새벽 → 낮으로 변화합니다.   
마지막 스테이지에서는 눈 효과가 강화되어   
게임 분위기를 연출합니다.

<br>

## 🎥 Camera System

탑다운 기반 아이소메트릭 뷰로 카메라를 구성하여  
플레이어와 전장을 한눈에 파악할 수 있도록 설계했습니다.  

카메라와 플레이어 사이의 장애물을 Raycast로 감지하여   
해당 오브젝트를 투명화하는 Camera Occlusion 시스템을 구현했습니다.  

몬스터 타격 시 카메라 흔들림(Camera Shake)을 적용하여   
전투 타격감을 강화했습니다.

## ⏳ Async Scene Loading

씬 전환 시 로딩 시간을 개선하기 위해 비동기 씬 로딩을 사용했습니다.  

씬 로딩 중에는 로딩 오버레이 UI를 표시하여   
플레이어가 자연스럽게 게임 시작을 기다릴 수 있도록 구현했습니다.

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
- Mixamo: [https://www.mixamo.com/]
- Flaticon: [https://www.flaticon.com/kr/]
- ChatGPT, Gemini AI를 사용하여 컷씬 이미지 생성
