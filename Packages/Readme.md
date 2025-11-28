# Walkable Vehivle Floor
## 概要
Walkable Vehivle Floorは、移動する車両内でStationの安定性を得つつ自由に移動する機能を提供します

# 目次
- [ビルドプロセス](#ビルドプロセス)
- [PlayerObject](#PlayerObject)
- [コンポーネント](#コンポーネント)
	- [CatchCollider_Vehicle](#catchcollider_vehicle)
	- [FloorStationController](#floorstationcontroller)
	- [VehicleInSideSeatMNG](#vehicleinsideseatmng)
 - [プレハブ](#prefab)
# ビルドプロセス
参照を同期する必要からビルドプロセスにて全Catcherを配列登録しています。

## walkableVehicleFloorBuildProcess
- 参照同期のためのリストや参照を作ります
- VehicleInSideSeatMNGを探します
- CatchCollider_Vehicleを探し、リストを作りVehicleInSideSeatMNGに渡します。CatchCollider_VehicleにはVehicleInSideSeatMNGとindexを渡しておきます（同期用）
- CatchCollider_Vehicleに設定されたinVehicleColliderの親をVehicleInSideSeatMNGとし、位置・回転を0にして無効化しておきます。

# PlayerObject
- movableSeatはPlayerObjectです。値の保存は行わず、インスタンス参加人数分用意するため用いています。

# コンポーネント
## CatchCollider_Vehicle

車両側コンポーネントです。TriggerコライダーにPlayerChaserが接触すると、プレイヤーをStationに乗せ、移動に関わる判定を切り離します。

### 仕様
プレイヤーの位置はinVehicleColliderオブジェクトとの相対位置=vehicleObjectとの相対位置となります。

プレイヤーが接触すると、その時点のvehicleObjectとの相対位置が計算され、VehicleInSideSeatMNG下のinVehicleCollider内の同じ相対位置に判定用のオブジェクトが出現します。

車両に乗っているPlayerが触れるか、AutoCatchが無効な場合は、触れることでInteract判定が有効となり、Interactで乗車となります。

### 使用方法
1. 任意の車両オブジェクト下にCatchCollider_Vehicleの付いたGameObjectを新規作成
2. CatchCollider_Vehicleのコライダーを任意の範囲に調整
	1. 必要であればCatchCollider_Vehicle.vehicleObjectを設定
3. CatchCollider_Vehicle.vehicleObjectと同じ場所にGameObjectを新規作成しCatchCollider_Vehicle.inVehicleColliderへ割り当て
4. CatchCollider_Vehicle.inVehicleCollider下に床・壁形状となるコライダーを作成

|設定値|概要|
|---:|:---|
vehicleObject|位置基準となるオブジェクト。nullであればCatchCollider_Vehicleの付いてるObject自身
inVehicleCollider|車両内の床形状となるオブジェクト

## FloorStationController

本パッケージで用いるStationの制御コンポーネントです。

### 仕様

ユーザー側での変更を想定した設定値はまだありません。

|設定値|概要|
|---:|:---|
preset_Manager|同期等の管理を行うManager
preset_sittingTransform|Stationの着座位置。車両に乗っている間は操作に応じてこれが動きます
seatedSetter|SDK2アバター用にSeatedを有効化するためのAnimator。VRCStationはSeatedをUdonで制御できないためAnimatorを用いています。

|関数|概要|同期|
|---:|:---|:---|
startSeating|指定されたIDの車両に乗車します。位置が遠いと無限に落下することになります。|内部実行
PlayerExitBounds_force|強制的に乗車状態を解除します|内部実行
PlayerExitBounds|指定されたIDの車両から降ります。指定車両に乗っていなければ何もしません。|内部実行
changeStationFallback|SDKアバターはSeated=falseなStationに乗るとアニメーションが正常に動作しないため、Animatorを介してSeated=trueに変更します。Toggle動作です。なおSDK3アバターはSeated=trueなステーションに乗ると腰から下を固めていない場合IKの解決がうまく行かなくなります。|内部実行

## VehicleInSideSeatMNG

ユーザー側での変更を想定した設定値はありません。

### 仕様

|設定値|概要|
|---:|:---|
preset_playerChaser|Playerに合わせて移動するオブジェクトです。Utilityに含まれる他スクリプトでも前提となっています。nullでは動作しません。

|関数|概要|同期|
|---:|:---|:---|
EnterOnVehicle|CatchCollider_Vehicleから呼び出され、プレイヤーが既に乗っていなければFloorStationController.startSeatingを呼び出して	乗車処理を開始します。|内部実行
ForcedRidingOnVehicle|現在の乗車状態を解除して、指定車両へプレイヤーを乗車させます。|内部実行
Exit|CatchCollider_Vehicleから呼び出され、FloorStationController.PlayerExitBoundsを呼び出して指定されたIDの車両からプレイヤーを降ろします。|内部実行
changeStationFallback|FloorStationController.changeStationFallbackを呼び出します。|内部実行

関数はいずれもLocalPlayerのFloorStationControllerを対象とします。

## WakableFloorSDK2FallBack

StationをSDK2対応動作に切り替えるためのコンポーネントです。

|設定値|概要|
|---:|:---|
preset_SeatMNG|VehicleInSideSeatMNGを設定してください。

# Prefab
## MovableSeatPrefab

### 内部図
```
    MovableSeatPrefab
    └── SeatMNG --------------- [VehicleInSideSeatMNG.u#]
        ├── movableSeat ------- [--> frou01.Utilty]
        └── WakableFloorSDK2FallBack ------- [WakableFloorSDK2FallBack.u#]
```
### 参照図
```
    ()は実行時自動参照
    MovableSeatPrefab [VehicleInSideSeatMNG.u#]
	↑					└── (movableSeat)
	↑
	WakableFloorSDK2FallBack[WakableFloorSDK2FallBack.u#]
```
### 使用方法
0. root下にMovableSeatPrefabを設置する