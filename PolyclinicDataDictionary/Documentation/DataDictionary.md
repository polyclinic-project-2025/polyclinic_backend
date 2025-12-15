# Diccionario de Datos - Sistema Polyclinic

**Generado:** 2025-12-15 02:30:37

---

## Tabla de Contenidos

- [ConsultationDerivation](#consultationderivation)
- [ConsultationReferral](#consultationreferral)
- [Department](#department)
- [DepartmentHead](#departmenthead)
- [Derivation](#derivation)
- [Doctor](#doctor)
- [EmergencyRoom](#emergencyroom)
- [EmergencyRoomCare](#emergencyroomcare)
- [Employee](#employee)
- [ExternalMedicalPost](#externalmedicalpost)
- [Medication](#medication)
- [MedicationDerivation](#medicationderivation)
- [MedicationEmergency](#medicationemergency)
- [MedicationReferral](#medicationreferral)
- [MedicationRequest](#medicationrequest)
- [Nurse](#nurse)
- [Patient](#patient)
- [Referral](#referral)
- [StockDepartment](#stockdepartment)
- [WarehouseManager](#warehousemanager)
- [WarehouseRequest](#warehouserequest)

---

## ConsultationDerivation

**Clase:** `ConsultationDerivation`
**Esquema:** `public`
**Tabla:** `ConsultationDerivation`

### Descripción
Consultas realizadas a pacientes derivados.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| ConsultationDerivationId | uuid | No | ✓ |  | Identificador de ConsultationDerivation |
| DateTimeCDer | timestamp with time zone | No |  |  | DateTimeCDer |
| DepartmentHeadId | uuid | No |  | ✓ | Identificador de DepartmentHead |
| DerivationId | uuid | No |  | ✓ | Identificador de Derivation |
| Diagnosis | character varying(1000) | No |  |  | Diagnosis (Max: 1000) |
| DoctorId | uuid | No |  | ✓ | Identificador de Doctor |
| PatientId | uuid | Sí |  | ✓ | Identificador de Patient |

### Clave Primaria

- **Nombre:** `PK_ConsultationDerivation`
- **Columnas:** `ConsultationDerivationId`

### Claves Foráneas

- **FK_ConsultationDerivation_DepartmentHead_DepartmentHeadId**
  - Columnas: `DepartmentHeadId`
  - Referencia: `DepartmentHead` (`DepartmentHeadId`)
  - Acción al eliminar: `Restrict`

- **FK_ConsultationDerivation_Derivation_DerivationId**
  - Columnas: `DerivationId`
  - Referencia: `Derivation` (`DerivationId`)
  - Acción al eliminar: `Restrict`

- **FK_ConsultationDerivation_Doctor_DoctorId**
  - Columnas: `DoctorId`
  - Referencia: `Doctor` (`EmployeeId`)
  - Acción al eliminar: `Restrict`

- **FK_ConsultationDerivation_Patient_PatientId**
  - Columnas: `PatientId`
  - Referencia: `Patient` (`PatientId`)
  - Acción al eliminar: `ClientSetNull`

### Índices

- **IX_ConsultationDerivation_DepartmentHeadId**
  - Columnas: `DepartmentHeadId`
  - Tipo: No único

- **IX_ConsultationDerivation_DerivationId**
  - Columnas: `DerivationId`
  - Tipo: No único

- **IX_ConsultationDerivation_PatientId**
  - Columnas: `PatientId`
  - Tipo: No único

- **IX_ConsultationDerivation_DoctorId_DerivationId_DateTimeCDer**
  - Columnas: `DoctorId`, `DerivationId`, `DateTimeCDer`
  - Tipo: Único

### Relaciones

- **DepartmentHead** → `DepartmentHead` (Muchos a Uno)
- **Derivation** → `Derivation` (Muchos a Uno)
- **Doctor** → `Doctor` (Muchos a Uno)
- **MedicationDerivations** → `MedicationDerivation` (Uno a Muchos)

---

## ConsultationReferral

**Clase:** `ConsultationReferral`
**Esquema:** `public`
**Tabla:** `ConsultationReferral`

### Descripción
Consultas realizadas a pacientes remitidos.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| ConsultationReferralId | uuid | No | ✓ |  | Identificador de ConsultationReferral |
| DateTimeCRem | timestamp with time zone | No |  |  | Fecha y hora de la consulta |
| DepartmentHeadId | uuid | No |  | ✓ | Identificador de DepartmentHead |
| Diagnosis | character varying(1000) | No |  |  | Diagnóstico de la consulta (Max: 1000) |
| DoctorId | uuid | No |  | ✓ | Identificador de Doctor |
| PatientId | uuid | Sí |  | ✓ | Identificador de Patient |
| ReferralId | uuid | No |  | ✓ | Identificador de Referral |

### Clave Primaria

- **Nombre:** `PK_ConsultationReferral`
- **Columnas:** `ConsultationReferralId`

### Claves Foráneas

- **FK_ConsultationReferral_DepartmentHead_DepartmentHeadId**
  - Columnas: `DepartmentHeadId`
  - Referencia: `DepartmentHead` (`DepartmentHeadId`)
  - Acción al eliminar: `Restrict`

- **FK_ConsultationReferral_Doctor_DoctorId**
  - Columnas: `DoctorId`
  - Referencia: `Doctor` (`EmployeeId`)
  - Acción al eliminar: `Restrict`

- **FK_ConsultationReferral_Patient_PatientId**
  - Columnas: `PatientId`
  - Referencia: `Patient` (`PatientId`)
  - Acción al eliminar: `ClientSetNull`

- **FK_ConsultationReferral_Referral_ReferralId**
  - Columnas: `ReferralId`
  - Referencia: `Referral` (`ReferralId`)
  - Acción al eliminar: `Restrict`

### Índices

- **IX_ConsultationReferral_DepartmentHeadId**
  - Columnas: `DepartmentHeadId`
  - Tipo: No único

- **IX_ConsultationReferral_PatientId**
  - Columnas: `PatientId`
  - Tipo: No único

- **IX_ConsultationReferral_ReferralId**
  - Columnas: `ReferralId`
  - Tipo: No único

- **IX_ConsultationReferral_DoctorId_ReferralId_DateTimeCRem**
  - Columnas: `DoctorId`, `ReferralId`, `DateTimeCRem`
  - Tipo: Único

### Relaciones

- **DepartmentHead** → `DepartmentHead` (Muchos a Uno)
- **Doctor** → `Doctor` (Muchos a Uno)
- **MedicationReferrals** → `MedicationReferral` (Uno a Muchos)
- **Referral** → `Referral` (Muchos a Uno)

---

## Department

**Clase:** `Department`
**Esquema:** `public`
**Tabla:** `Department`

### Descripción
Catálogo de departamentos médicos.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| DepartmentId | uuid | No | ✓ |  | Identificador de Department |
| Name | character varying(200) | No |  |  | Nombre (Max: 200) |

### Clave Primaria

- **Nombre:** `PK_Department`
- **Columnas:** `DepartmentId`

### Relaciones

- **DepartmentHeads** → `DepartmentHead` (Uno a Muchos)
- **DerivationsFrom** → `Derivation` (Uno a Muchos)
- **DerivationsTo** → `Derivation` (Uno a Muchos)
- **Doctors** → `Doctor` (Uno a Muchos)
- **Referrals** → `Referral` (Uno a Muchos)
- **StockDepartments** → `StockDepartment` (Uno a Muchos)
- **WarehouseRequests** → `WarehouseRequest` (Uno a Muchos)

---

## DepartmentHead

**Clase:** `DepartmentHead`
**Esquema:** `public`
**Tabla:** `DepartmentHead`

### Descripción
Asignación de jefes de departamento.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| DepartmentHeadId | uuid | No | ✓ |  | Identificador de DepartmentHead |
| AssignedAt | timestamp with time zone | No |  |  | AssignedAt |
| DepartmentId | uuid | No |  | ✓ | Identificador de Department |
| DoctorId | uuid | No |  | ✓ | Identificador de Doctor |

### Clave Primaria

- **Nombre:** `PK_DepartmentHead`
- **Columnas:** `DepartmentHeadId`

### Claves Foráneas

- **FK_DepartmentHead_Department_DepartmentId**
  - Columnas: `DepartmentId`
  - Referencia: `Department` (`DepartmentId`)
  - Acción al eliminar: `Restrict`

- **FK_DepartmentHead_Doctor_DoctorId**
  - Columnas: `DoctorId`
  - Referencia: `Doctor` (`EmployeeId`)
  - Acción al eliminar: `Restrict`

### Índices

- **IX_DepartmentHead_DepartmentId**
  - Columnas: `DepartmentId`
  - Tipo: No único

- **IX_DepartmentHead_DoctorId**
  - Columnas: `DoctorId`
  - Tipo: No único

### Relaciones

- **ConsultationDerivations** → `ConsultationDerivation` (Uno a Muchos)
- **ConsultationReferrals** → `ConsultationReferral` (Uno a Muchos)
- **Department** → `Department` (Muchos a Uno)
- **Doctor** → `Doctor` (Muchos a Uno)
- **WarehouseRequests** → `WarehouseRequest` (Uno a Muchos)

---

## Derivation

**Clase:** `Derivation`
**Esquema:** `public`
**Tabla:** `Derivation`

### Descripción
Registra derivaciones entre departamentos internos.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| DerivationId | uuid | No | ✓ |  | Identificador de Derivation |
| DateTimeDer | timestamp with time zone | No |  |  | Fecha y hora de la derivación |
| DepartmentFromId | uuid | No |  | ✓ | Identificador de DepartmentFrom |
| DepartmentToId | uuid | No |  | ✓ | Identificador de DepartmentTo |
| PatientId | uuid | No |  | ✓ | Identificador de Patient |

### Clave Primaria

- **Nombre:** `PK_Derivation`
- **Columnas:** `DerivationId`

### Claves Foráneas

- **FK_Derivation_Department_DepartmentFromId**
  - Columnas: `DepartmentFromId`
  - Referencia: `Department` (`DepartmentId`)
  - Acción al eliminar: `Restrict`

- **FK_Derivation_Department_DepartmentToId**
  - Columnas: `DepartmentToId`
  - Referencia: `Department` (`DepartmentId`)
  - Acción al eliminar: `Restrict`

- **FK_Derivation_Patient_PatientId**
  - Columnas: `PatientId`
  - Referencia: `Patient` (`PatientId`)
  - Acción al eliminar: `Restrict`

### Índices

- **IX_Derivation_DepartmentToId**
  - Columnas: `DepartmentToId`
  - Tipo: No único

- **IX_Derivation_PatientId**
  - Columnas: `PatientId`
  - Tipo: No único

- **IX_Derivation_DepartmentFromId_DateTimeDer_PatientId**
  - Columnas: `DepartmentFromId`, `DateTimeDer`, `PatientId`
  - Tipo: Único

### Relaciones

- **ConsultationDerivations** → `ConsultationDerivation` (Uno a Muchos)
- **DepartmentFrom** → `Department` (Muchos a Uno)
- **DepartmentTo** → `Department` (Muchos a Uno)
- **Patient** → `Patient` (Muchos a Uno)

---

## Doctor

**Clase:** `Doctor`
**Esquema:** `public`
**Tabla:** `Doctor`

### Descripción
Registra datos de los médicos empleados.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| EmployeeId | uuid | No | ✓ | ✓ | Identificador de Employee |
| DepartmentId | uuid | No |  | ✓ | Identificador de Department |
| EmploymentStatus | character varying(50) | No |  |  | EmploymentStatus (Max: 50) |
| Identification | character varying(50) | No |  |  | Número de identificación (Max: 50) |
| Name | character varying(200) | No |  |  | Nombre (Max: 200) |
| UserId | text | Sí |  | ✓ | Identificador de User |

### Clave Primaria

- **Nombre:** `PK_Employee`
- **Columnas:** `EmployeeId`

### Claves Foráneas

- **FK_Employee_Users_UserId**
  - Columnas: `UserId`
  - Referencia: `Users` (`Id`)
  - Acción al eliminar: `SetNull`

- **FK_Doctor_Department_DepartmentId**
  - Columnas: `DepartmentId`
  - Referencia: `Department` (`DepartmentId`)
  - Acción al eliminar: `Restrict`

- **FK_Doctor_Employee_EmployeeId**
  - Columnas: `EmployeeId`
  - Referencia: `Employee` (`EmployeeId`)
  - Acción al eliminar: `Cascade`

### Índices

- **IX_Employee_Identification**
  - Columnas: `Identification`
  - Tipo: Único

- **IX_Employee_UserId**
  - Columnas: `UserId`
  - Tipo: Único

- **IX_Doctor_DepartmentId**
  - Columnas: `DepartmentId`
  - Tipo: No único

### Relaciones

- **ConsultationDerivations** → `ConsultationDerivation` (Uno a Muchos)
- **ConsultationReferrals** → `ConsultationReferral` (Uno a Muchos)
- **Department** → `Department` (Muchos a Uno)
- **DepartmentHeads** → `DepartmentHead` (Uno a Muchos)
- **EmergencyRooms** → `EmergencyRoom` (Uno a Muchos)

---

## EmergencyRoom

**Clase:** `EmergencyRoom`
**Esquema:** `public`
**Tabla:** `EmergencyRoom`

### Descripción
Guardias de emergencias asignadas a médicos.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| EmergencyRoomId | uuid | No | ✓ |  | Identificador de EmergencyRoom |
| DoctorId | uuid | No |  | ✓ | Identificador de Doctor |
| GuardDate | date | No |  |  | GuardDate |

### Clave Primaria

- **Nombre:** `PK_EmergencyRoom`
- **Columnas:** `EmergencyRoomId`

### Claves Foráneas

- **FK_EmergencyRoom_Doctor_DoctorId**
  - Columnas: `DoctorId`
  - Referencia: `Doctor` (`EmployeeId`)
  - Acción al eliminar: `Restrict`

### Índices

- **IX_EmergencyRoom_DoctorId_GuardDate**
  - Columnas: `DoctorId`, `GuardDate`
  - Tipo: Único

### Relaciones

- **Doctor** → `Doctor` (Muchos a Uno)
- **EmergencyRoomCares** → `EmergencyRoomCare` (Uno a Muchos)

---

## EmergencyRoomCare

**Clase:** `EmergencyRoomCare`
**Esquema:** `public`
**Tabla:** `EmergencyRoomCare`

### Descripción
Atenciones realizadas en emergencias.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| EmergencyRoomCareId | uuid | No | ✓ |  | Identificador de EmergencyRoomCare |
| CareDate | timestamp with time zone | No |  |  | CareDate |
| Diagnosis | character varying(1000) | No |  |  | Diagnosis (Max: 1000) |
| EmergencyRoomId | uuid | No |  | ✓ | Identificador de EmergencyRoom |
| PatientId | uuid | No |  | ✓ | Identificador de Patient |

### Clave Primaria

- **Nombre:** `PK_EmergencyRoomCare`
- **Columnas:** `EmergencyRoomCareId`

### Claves Foráneas

- **FK_EmergencyRoomCare_EmergencyRoom_EmergencyRoomId**
  - Columnas: `EmergencyRoomId`
  - Referencia: `EmergencyRoom` (`EmergencyRoomId`)
  - Acción al eliminar: `Restrict`

- **FK_EmergencyRoomCare_Patient_PatientId**
  - Columnas: `PatientId`
  - Referencia: `Patient` (`PatientId`)
  - Acción al eliminar: `Restrict`

### Índices

- **IX_EmergencyRoomCare_EmergencyRoomId**
  - Columnas: `EmergencyRoomId`
  - Tipo: No único

- **IX_EmergencyRoomCare_PatientId**
  - Columnas: `PatientId`
  - Tipo: No único

- **IX_EmergencyRoomCare_CareDate_PatientId**
  - Columnas: `CareDate`, `PatientId`
  - Tipo: Único

### Relaciones

- **EmergencyRoom** → `EmergencyRoom` (Muchos a Uno)
- **MedicationEmergencies** → `MedicationEmergency` (Uno a Muchos)
- **Patient** → `Patient` (Muchos a Uno)

---

## Employee

**Clase:** `Employee`
**Esquema:** `public`
**Tabla:** `Employee`

### Descripción
Tabla base para todos los empleados del sistema.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| EmployeeId | uuid | No | ✓ | ✓ | Identificador de Employee |
| EmploymentStatus | character varying(50) | No |  |  | Estado laboral del empleado (Max: 50) |
| Identification | character varying(50) | No |  |  | Número de identificación (Max: 50) |
| Name | character varying(200) | No |  |  | Nombre (Max: 200) |
| UserId | text | Sí |  | ✓ | Identificador de User |

### Clave Primaria

- **Nombre:** `PK_Employee`
- **Columnas:** `EmployeeId`

### Claves Foráneas

- **FK_Employee_Users_UserId**
  - Columnas: `UserId`
  - Referencia: `Users` (`Id`)
  - Acción al eliminar: `SetNull`

### Índices

- **IX_Employee_Identification**
  - Columnas: `Identification`
  - Tipo: Único

- **IX_Employee_UserId**
  - Columnas: `UserId`
  - Tipo: Único

---

## ExternalMedicalPost

**Clase:** `ExternalMedicalPost`
**Esquema:** `public`
**Tabla:** `ExternalMedicalPost`

### Descripción
Puestos médicos externos que remiten pacientes.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| ExternalMedicalPostId | uuid | No | ✓ |  | Identificador de ExternalMedicalPost |
| Name | character varying(200) | No |  |  | Nombre (Max: 200) |

### Clave Primaria

- **Nombre:** `PK_ExternalMedicalPost`
- **Columnas:** `ExternalMedicalPostId`

---

## Medication

**Clase:** `Medication`
**Esquema:** `public`
**Tabla:** `Medication`

### Descripción
Catálogo de medicamentos disponibles.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| MedicationId | uuid | No | ✓ |  | Identificador de Medication |
| BatchNumber | character varying(100) | No |  |  | Número de lote (Max: 100) |
| CommercialCompany | character varying(100) | No |  |  | CommercialCompany (Max: 100) |
| CommercialName | character varying(100) | No |  |  | CommercialName (Max: 100) |
| ExpirationDate | date | No |  |  | Fecha de vencimiento del medicamento |
| Format | character varying(100) | No |  |  | Format (Max: 100) |
| MaxQuantityNurse | integer | No |  |  | MaxQuantityNurse |
| MaxQuantityWarehouse | integer | No |  |  | MaxQuantityWarehouse |
| MinQuantityNurse | integer | No |  |  | MinQuantityNurse |
| MinQuantityWarehouse | integer | No |  |  | MinQuantityWarehouse |
| QuantityNurse | integer | No |  |  | Cantidad disponible en enfermería |
| QuantityWarehouse | integer | No |  |  | QuantityWarehouse |
| ScientificName | character varying(100) | No |  |  | ScientificName (Max: 100) |

### Clave Primaria

- **Nombre:** `PK_Medication`
- **Columnas:** `MedicationId`

### Relaciones

- **MedicationDerivations** → `MedicationDerivation` (Uno a Muchos)
- **MedicationEmergencies** → `MedicationEmergency` (Uno a Muchos)
- **MedicationReferrals** → `MedicationReferral` (Uno a Muchos)
- **MedicationRequests** → `MedicationRequest` (Uno a Muchos)
- **StockDepartments** → `StockDepartment` (Uno a Muchos)

---

## MedicationDerivation

**Clase:** `MedicationDerivation`
**Esquema:** `public`
**Tabla:** `MedicationDerivation`

### Descripción
Entidad del sistema Polyclinic.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| MedicationDerivationId | uuid | No | ✓ |  | Identificador de MedicationDerivation |
| ConsultationDerivationId | uuid | No |  | ✓ | Identificador de ConsultationDerivation |
| MedicationId | uuid | No |  | ✓ | Identificador de Medication |
| Quantity | integer | No |  |  | Quantity |

### Clave Primaria

- **Nombre:** `PK_MedicationDerivation`
- **Columnas:** `MedicationDerivationId`

### Claves Foráneas

- **FK_MedicationDerivation_ConsultationDerivation_ConsultationDer~**
  - Columnas: `ConsultationDerivationId`
  - Referencia: `ConsultationDerivation` (`ConsultationDerivationId`)
  - Acción al eliminar: `Restrict`

- **FK_MedicationDerivation_Medication_MedicationId**
  - Columnas: `MedicationId`
  - Referencia: `Medication` (`MedicationId`)
  - Acción al eliminar: `Restrict`

### Índices

- **IX_MedicationDerivation_MedicationId**
  - Columnas: `MedicationId`
  - Tipo: No único

- **IX_MedicationDerivation_ConsultationDerivationId_MedicationId**
  - Columnas: `ConsultationDerivationId`, `MedicationId`
  - Tipo: Único

### Relaciones

- **ConsultationDerivation** → `ConsultationDerivation` (Muchos a Uno)
- **Medication** → `Medication` (Muchos a Uno)

---

## MedicationEmergency

**Clase:** `MedicationEmergency`
**Esquema:** `public`
**Tabla:** `MedicationEmergency`

### Descripción
Entidad del sistema Polyclinic.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| MedicationEmergencyId | uuid | No | ✓ |  | Identificador de MedicationEmergency |
| EmergencyRoomCareId | uuid | No |  | ✓ | Identificador de EmergencyRoomCare |
| MedicationId | uuid | No |  | ✓ | Identificador de Medication |
| Quantity | integer | No |  |  | Quantity |

### Clave Primaria

- **Nombre:** `PK_MedicationEmergency`
- **Columnas:** `MedicationEmergencyId`

### Claves Foráneas

- **FK_MedicationEmergency_EmergencyRoomCare_EmergencyRoomCareId**
  - Columnas: `EmergencyRoomCareId`
  - Referencia: `EmergencyRoomCare` (`EmergencyRoomCareId`)
  - Acción al eliminar: `Restrict`

- **FK_MedicationEmergency_Medication_MedicationId**
  - Columnas: `MedicationId`
  - Referencia: `Medication` (`MedicationId`)
  - Acción al eliminar: `Restrict`

### Índices

- **IX_MedicationEmergency_MedicationId**
  - Columnas: `MedicationId`
  - Tipo: No único

- **IX_MedicationEmergency_EmergencyRoomCareId_MedicationId**
  - Columnas: `EmergencyRoomCareId`, `MedicationId`
  - Tipo: Único

### Relaciones

- **EmergencyRoomCare** → `EmergencyRoomCare` (Muchos a Uno)
- **Medication** → `Medication` (Muchos a Uno)

---

## MedicationReferral

**Clase:** `MedicationReferral`
**Esquema:** `public`
**Tabla:** `MedicationReferral`

### Descripción
Entidad del sistema Polyclinic.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| MedicationReferralId | uuid | No | ✓ |  | Identificador de MedicationReferral |
| ConsultationReferralId | uuid | No |  | ✓ | Identificador de ConsultationReferral |
| MedicationId | uuid | No |  | ✓ | Identificador de Medication |
| Quantity | integer | No |  |  | Quantity |

### Clave Primaria

- **Nombre:** `PK_MedicationReferral`
- **Columnas:** `MedicationReferralId`

### Claves Foráneas

- **FK_MedicationReferral_ConsultationReferral_ConsultationReferra~**
  - Columnas: `ConsultationReferralId`
  - Referencia: `ConsultationReferral` (`ConsultationReferralId`)
  - Acción al eliminar: `Restrict`

- **FK_MedicationReferral_Medication_MedicationId**
  - Columnas: `MedicationId`
  - Referencia: `Medication` (`MedicationId`)
  - Acción al eliminar: `Restrict`

### Índices

- **IX_MedicationReferral_MedicationId**
  - Columnas: `MedicationId`
  - Tipo: No único

- **IX_MedicationReferral_ConsultationReferralId_MedicationId**
  - Columnas: `ConsultationReferralId`, `MedicationId`
  - Tipo: Único

### Relaciones

- **ConsultationReferral** → `ConsultationReferral` (Muchos a Uno)
- **Medication** → `Medication` (Muchos a Uno)

---

## MedicationRequest

**Clase:** `MedicationRequest`
**Esquema:** `public`
**Tabla:** `MedicationRequest`

### Descripción
Entidad del sistema Polyclinic.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| MedicationRequestId | uuid | No | ✓ |  | Identificador de MedicationRequest |
| MedicationId | uuid | No |  | ✓ | Identificador de Medication |
| Quantity | integer | No |  |  | Quantity |
| WarehouseRequestId | uuid | No |  | ✓ | Identificador de WarehouseRequest |

### Clave Primaria

- **Nombre:** `PK_MedicationRequest`
- **Columnas:** `MedicationRequestId`

### Claves Foráneas

- **FK_MedicationRequest_Medication_MedicationId**
  - Columnas: `MedicationId`
  - Referencia: `Medication` (`MedicationId`)
  - Acción al eliminar: `Restrict`

- **FK_MedicationRequest_WarehouseRequest_WarehouseRequestId**
  - Columnas: `WarehouseRequestId`
  - Referencia: `WarehouseRequest` (`WarehouseRequestId`)
  - Acción al eliminar: `Restrict`

### Índices

- **IX_MedicationRequest_MedicationId**
  - Columnas: `MedicationId`
  - Tipo: No único

- **IX_MedicationRequest_WarehouseRequestId_MedicationId**
  - Columnas: `WarehouseRequestId`, `MedicationId`
  - Tipo: Único

### Relaciones

- **Medication** → `Medication` (Muchos a Uno)
- **WarehouseRequest** → `WarehouseRequest` (Muchos a Uno)

---

## Nurse

**Clase:** `Nurse`
**Esquema:** `public`
**Tabla:** `Nurse`

### Descripción
Registra datos de enfermeros empleados.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| EmployeeId | uuid | No | ✓ | ✓ | Identificador de Employee |
| EmploymentStatus | character varying(50) | No |  |  | EmploymentStatus (Max: 50) |
| Identification | character varying(50) | No |  |  | Número de identificación (Max: 50) |
| Name | character varying(200) | No |  |  | Nombre (Max: 200) |
| UserId | text | Sí |  | ✓ | Identificador de User |

### Clave Primaria

- **Nombre:** `PK_Employee`
- **Columnas:** `EmployeeId`

### Claves Foráneas

- **FK_Employee_Users_UserId**
  - Columnas: `UserId`
  - Referencia: `Users` (`Id`)
  - Acción al eliminar: `SetNull`

- **FK_Nurse_Employee_EmployeeId**
  - Columnas: `EmployeeId`
  - Referencia: `Employee` (`EmployeeId`)
  - Acción al eliminar: `Cascade`

### Índices

- **IX_Employee_Identification**
  - Columnas: `Identification`
  - Tipo: Único

- **IX_Employee_UserId**
  - Columnas: `UserId`
  - Tipo: Único

---

## Patient

**Clase:** `Patient`
**Esquema:** `public`
**Tabla:** `Patient`

### Descripción
Almacena información de los pacientes del sistema.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| PatientId | uuid | No | ✓ |  | Identificador de Patient |
| Address | character varying(500) | No |  |  | Dirección de residencia (Max: 500) |
| Age | integer | No |  |  | Edad del paciente |
| Contact | character varying(100) | No |  |  | Información de contacto (Max: 100) |
| Identification | character varying(50) | No |  |  | Número de identificación (Max: 50) |
| Name | character varying(200) | No |  |  | Nombre (Max: 200) |
| UserId | text | Sí |  | ✓ | Identificador de User |

### Clave Primaria

- **Nombre:** `PK_Patient`
- **Columnas:** `PatientId`

### Claves Foráneas

- **FK_Patient_Users_UserId**
  - Columnas: `UserId`
  - Referencia: `Users` (`Id`)
  - Acción al eliminar: `SetNull`

### Índices

- **IX_Patient_Identification**
  - Columnas: `Identification`
  - Tipo: Único

- **IX_Patient_UserId**
  - Columnas: `UserId`
  - Tipo: Único

### Relaciones

- **ConsultationDerivations** → `ConsultationDerivation` (Uno a Muchos)
- **ConsultationReferrals** → `ConsultationReferral` (Uno a Muchos)
- **Derivations** → `Derivation` (Uno a Muchos)
- **EmergencyRoomCares** → `EmergencyRoomCare` (Uno a Muchos)
- **Referrals** → `Referral` (Uno a Muchos)

---

## Referral

**Clase:** `Referral`
**Esquema:** `public`
**Tabla:** `Referral`

### Descripción
Registra remisiones desde puestos médicos externos.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| ReferralId | uuid | No | ✓ |  | Identificador de Referral |
| DateTimeRem | timestamp with time zone | No |  |  | Fecha y hora de la remisión |
| DepartmentToId | uuid | No |  | ✓ | Identificador de DepartmentTo |
| ExternalMedicalPostId | uuid | No |  | ✓ | Identificador de ExternalMedicalPost |
| PatientId | uuid | No |  | ✓ | Identificador de Patient |

### Clave Primaria

- **Nombre:** `PK_Referral`
- **Columnas:** `ReferralId`

### Claves Foráneas

- **FK_Referral_Department_DepartmentToId**
  - Columnas: `DepartmentToId`
  - Referencia: `Department` (`DepartmentId`)
  - Acción al eliminar: `Restrict`

- **FK_Referral_ExternalMedicalPost_ExternalMedicalPostId**
  - Columnas: `ExternalMedicalPostId`
  - Referencia: `ExternalMedicalPost` (`ExternalMedicalPostId`)
  - Acción al eliminar: `Restrict`

- **FK_Referral_Patient_PatientId**
  - Columnas: `PatientId`
  - Referencia: `Patient` (`PatientId`)
  - Acción al eliminar: `Restrict`

### Índices

- **IX_Referral_DepartmentToId**
  - Columnas: `DepartmentToId`
  - Tipo: No único

- **IX_Referral_ExternalMedicalPostId**
  - Columnas: `ExternalMedicalPostId`
  - Tipo: No único

- **IX_Referral_PatientId_DateTimeRem_ExternalMedicalPostId**
  - Columnas: `PatientId`, `DateTimeRem`, `ExternalMedicalPostId`
  - Tipo: Único

### Relaciones

- **ConsultationReferrals** → `ConsultationReferral` (Uno a Muchos)
- **DepartmentTo** → `Department` (Muchos a Uno)
- **ExternalMedicalPost** → `ExternalMedicalPost` (Muchos a Uno)
- **Patient** → `Patient` (Muchos a Uno)

---

## StockDepartment

**Clase:** `StockDepartment`
**Esquema:** `public`
**Tabla:** `StockDepartment`

### Descripción
Entidad del sistema Polyclinic.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| StockDepartmentId | uuid | No | ✓ |  | Identificador de StockDepartment |
| DepartmentId | uuid | No |  | ✓ | Identificador de Department |
| MaxQuantity | integer | No |  |  | MaxQuantity |
| MedicationId | uuid | No |  | ✓ | Identificador de Medication |
| MinQuantity | integer | No |  |  | MinQuantity |
| Quantity | integer | No |  |  | Quantity |

### Clave Primaria

- **Nombre:** `PK_StockDepartment`
- **Columnas:** `StockDepartmentId`

### Claves Foráneas

- **FK_StockDepartment_Department_DepartmentId**
  - Columnas: `DepartmentId`
  - Referencia: `Department` (`DepartmentId`)
  - Acción al eliminar: `Restrict`

- **FK_StockDepartment_Medication_MedicationId**
  - Columnas: `MedicationId`
  - Referencia: `Medication` (`MedicationId`)
  - Acción al eliminar: `Restrict`

### Índices

- **IX_StockDepartment_MedicationId**
  - Columnas: `MedicationId`
  - Tipo: No único

- **IX_StockDepartment_DepartmentId_MedicationId**
  - Columnas: `DepartmentId`, `MedicationId`
  - Tipo: Único

### Relaciones

- **Department** → `Department` (Muchos a Uno)
- **Medication** → `Medication` (Muchos a Uno)

---

## WarehouseManager

**Clase:** `WarehouseManager`
**Esquema:** `public`
**Tabla:** `WarehouseManager`

### Descripción
Entidad del sistema Polyclinic.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| EmployeeId | uuid | No | ✓ | ✓ | Identificador de Employee |
| AssignedAt | timestamp with time zone | No |  |  | AssignedAt |
| EmploymentStatus | character varying(50) | No |  |  | EmploymentStatus (Max: 50) |
| Identification | character varying(50) | No |  |  | Número de identificación (Max: 50) |
| Name | character varying(200) | No |  |  | Nombre (Max: 200) |
| UserId | text | Sí |  | ✓ | Identificador de User |

### Clave Primaria

- **Nombre:** `PK_Employee`
- **Columnas:** `EmployeeId`

### Claves Foráneas

- **FK_Employee_Users_UserId**
  - Columnas: `UserId`
  - Referencia: `Users` (`Id`)
  - Acción al eliminar: `SetNull`

- **FK_WarehouseManager_Employee_EmployeeId**
  - Columnas: `EmployeeId`
  - Referencia: `Employee` (`EmployeeId`)
  - Acción al eliminar: `Cascade`

### Índices

- **IX_Employee_Identification**
  - Columnas: `Identification`
  - Tipo: Único

- **IX_Employee_UserId**
  - Columnas: `UserId`
  - Tipo: Único

### Relaciones

- **WarehouseRequests** → `WarehouseRequest` (Uno a Muchos)

---

## WarehouseRequest

**Clase:** `WarehouseRequest`
**Esquema:** `public`
**Tabla:** `WarehouseRequest`

### Descripción
Entidad del sistema Polyclinic.

### Columnas

| Columna | Tipo | Nullable | PK | FK | Descripción |
|---------|------|----------|----|----|-------------|
| WarehouseRequestId | uuid | No | ✓ |  | Identificador de WarehouseRequest |
| DepartmentHeadId | uuid | Sí |  | ✓ | Identificador de DepartmentHead |
| DepartmentId | uuid | No |  | ✓ | Identificador de Department |
| RequestDate | timestamp with time zone | No |  |  | RequestDate |
| Status | character varying(100) | No |  |  | Status (Max: 100) |
| WarehouseManagerId | uuid | No |  | ✓ | Identificador de WarehouseManager |

### Clave Primaria

- **Nombre:** `PK_WarehouseRequest`
- **Columnas:** `WarehouseRequestId`

### Claves Foráneas

- **FK_WarehouseRequest_DepartmentHead_DepartmentHeadId**
  - Columnas: `DepartmentHeadId`
  - Referencia: `DepartmentHead` (`DepartmentHeadId`)
  - Acción al eliminar: `ClientSetNull`

- **FK_WarehouseRequest_Department_DepartmentId**
  - Columnas: `DepartmentId`
  - Referencia: `Department` (`DepartmentId`)
  - Acción al eliminar: `Restrict`

- **FK_WarehouseRequest_WarehouseManager_WarehouseManagerId**
  - Columnas: `WarehouseManagerId`
  - Referencia: `WarehouseManager` (`EmployeeId`)
  - Acción al eliminar: `Restrict`

### Índices

- **IX_WarehouseRequest_DepartmentHeadId**
  - Columnas: `DepartmentHeadId`
  - Tipo: No único

- **IX_WarehouseRequest_DepartmentId**
  - Columnas: `DepartmentId`
  - Tipo: No único

- **IX_WarehouseRequest_WarehouseManagerId**
  - Columnas: `WarehouseManagerId`
  - Tipo: No único

- **IX_WarehouseRequest_RequestDate_DepartmentId**
  - Columnas: `RequestDate`, `DepartmentId`
  - Tipo: Único

### Relaciones

- **Department** → `Department` (Muchos a Uno)
- **MedicationRequests** → `MedicationRequest` (Uno a Muchos)
- **WarehouseManager** → `WarehouseManager` (Muchos a Uno)

---

## Resumen de Relaciones

| Tabla Origen | Relación | Tabla Destino | Tipo |
|--------------|----------|---------------|------|
| ConsultationDerivation | DepartmentHeadId | DepartmentHead | N:1 |
| ConsultationDerivation | DerivationId | Derivation | N:1 |
| ConsultationDerivation | DoctorId | Doctor | N:1 |
| ConsultationDerivation | PatientId | Patient | N:1 |
| ConsultationReferral | DepartmentHeadId | DepartmentHead | N:1 |
| ConsultationReferral | DoctorId | Doctor | N:1 |
| ConsultationReferral | PatientId | Patient | N:1 |
| ConsultationReferral | ReferralId | Referral | N:1 |
| DepartmentHead | DepartmentId | Department | N:1 |
| DepartmentHead | DoctorId | Doctor | N:1 |
| Derivation | DepartmentFromId | Department | N:1 |
| Derivation | DepartmentToId | Department | N:1 |
| Derivation | PatientId | Patient | N:1 |
| Doctor | UserId | Users | 1:1 |
| Doctor | DepartmentId | Department | N:1 |
| Doctor | EmployeeId | Employee | 1:1 |
| EmergencyRoom | DoctorId | Doctor | N:1 |
| EmergencyRoomCare | EmergencyRoomId | EmergencyRoom | N:1 |
| EmergencyRoomCare | PatientId | Patient | N:1 |
| Employee | UserId | Users | 1:1 |
| MedicationDerivation | ConsultationDerivationId | ConsultationDerivation | N:1 |
| MedicationDerivation | MedicationId | Medication | N:1 |
| MedicationEmergency | EmergencyRoomCareId | EmergencyRoomCare | N:1 |
| MedicationEmergency | MedicationId | Medication | N:1 |
| MedicationReferral | ConsultationReferralId | ConsultationReferral | N:1 |
| MedicationReferral | MedicationId | Medication | N:1 |
| MedicationRequest | MedicationId | Medication | N:1 |
| MedicationRequest | WarehouseRequestId | WarehouseRequest | N:1 |
| Nurse | UserId | Users | 1:1 |
| Nurse | EmployeeId | Employee | 1:1 |
| Patient | UserId | Users | 1:1 |
| Referral | DepartmentToId | Department | N:1 |
| Referral | ExternalMedicalPostId | ExternalMedicalPost | N:1 |
| Referral | PatientId | Patient | N:1 |
| StockDepartment | DepartmentId | Department | N:1 |
| StockDepartment | MedicationId | Medication | N:1 |
| WarehouseManager | UserId | Users | 1:1 |
| WarehouseManager | EmployeeId | Employee | 1:1 |
| WarehouseRequest | DepartmentHeadId | DepartmentHead | N:1 |
| WarehouseRequest | DepartmentId | Department | N:1 |
| WarehouseRequest | WarehouseManagerId | WarehouseManager | N:1 |

