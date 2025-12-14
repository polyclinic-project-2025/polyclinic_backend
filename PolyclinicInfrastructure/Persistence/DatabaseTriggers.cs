namespace PolyclinicInfrastructure.Persistence;

/// <summary>
/// Clase que contiene los scripts SQL para crear triggers y funciones de validación en la base de datos.
/// Estos triggers aseguran la integridad de datos a nivel de base de datos.
/// </summary>
public static class DatabaseTriggers
{
    /// <summary>
    /// Script SQL para crear la función que valida que el DepartmentHead y el Doctor de una ConsultationReferral
    /// pertenezcan al mismo departamento destino de la Referral asociada.
    /// </summary>
    public static string CreateConsultationReferralValidationFunction => @"
        CREATE OR REPLACE FUNCTION fn_validate_consultation_referral_department()
        RETURNS TRIGGER AS $$
        DECLARE
            v_department_head_dept_id UUID;
            v_doctor_dept_id UUID;
            v_referral_dept_to_id UUID;
        BEGIN
            -- Obtener el DepartmentId del DepartmentHead
            SELECT ""DepartmentId"" INTO v_department_head_dept_id
            FROM ""DepartmentHead""
            WHERE ""DepartmentHeadId"" = NEW.""DepartmentHeadId"";

            -- Obtener el DepartmentId del Doctor
            SELECT ""DepartmentId"" INTO v_doctor_dept_id
            FROM ""Doctor""
            WHERE ""EmployeeId"" = NEW.""DoctorId"";

            -- Obtener el DepartmentToId de la Referral
            SELECT ""DepartmentToId"" INTO v_referral_dept_to_id
            FROM ""Referral""
            WHERE ""ReferralId"" = NEW.""ReferralId"";

            -- Validar que el DepartmentHead pertenezca al departamento destino
            IF v_department_head_dept_id <> v_referral_dept_to_id THEN
                RAISE EXCEPTION 'El jefe de departamento debe pertenecer al mismo departamento destino de la remisión. DepartmentHead.DepartmentId: %, Referral.DepartmentToId: %', 
                    v_department_head_dept_id, v_referral_dept_to_id;
            END IF;

            -- Validar que el Doctor pertenezca al departamento destino
            IF v_doctor_dept_id <> v_referral_dept_to_id THEN
                RAISE EXCEPTION 'El doctor tratante debe pertenecer al mismo departamento destino de la remisión. Doctor.DepartmentId: %, Referral.DepartmentToId: %', 
                    v_doctor_dept_id, v_referral_dept_to_id;
            END IF;

            RETURN NEW;
        END;
        $$ LANGUAGE plpgsql;
    ";

    /// <summary>
    /// Script SQL para crear el trigger que ejecuta la validación antes de INSERT o UPDATE
    /// en la tabla ConsultationReferral.
    /// </summary>
    public static string CreateConsultationReferralValidationTrigger => @"
        DO $$
        BEGIN
            -- Eliminar el trigger si existe para recrearlo
            DROP TRIGGER IF EXISTS tr_consultation_referral_validate_department ON ""ConsultationReferral"";
            
            -- Crear el trigger
            CREATE TRIGGER tr_consultation_referral_validate_department
            BEFORE INSERT OR UPDATE ON ""ConsultationReferral""
            FOR EACH ROW
            EXECUTE FUNCTION fn_validate_consultation_referral_department();
        END $$;
    ";

    /// <summary>
    /// Script SQL para eliminar el trigger de validación de ConsultationReferral.
    /// </summary>
    public static string DropConsultationReferralValidationTrigger => @"
        DROP TRIGGER IF EXISTS tr_consultation_referral_validate_department ON ""ConsultationReferral"";
    ";

    /// <summary>
    /// Script SQL para eliminar la función de validación de ConsultationReferral.
    /// </summary>
    public static string DropConsultationReferralValidationFunction => @"
        DROP FUNCTION IF EXISTS fn_validate_consultation_referral_department();
    ";

    #region ConsultationDerivation Validation

    /// <summary>
    /// Script SQL para crear la función que valida que el DepartmentHead y el Doctor de una ConsultationDerivation
    /// pertenezcan al mismo departamento destino de la Derivation asociada.
    /// </summary>
    public static string CreateConsultationDerivationValidationFunction => @"
        CREATE OR REPLACE FUNCTION fn_validate_consultation_derivation_department()
        RETURNS TRIGGER AS $$
        DECLARE
            v_department_head_dept_id UUID;
            v_doctor_dept_id UUID;
            v_derivation_dept_to_id UUID;
        BEGIN
            -- Obtener el DepartmentId del DepartmentHead
            SELECT ""DepartmentId"" INTO v_department_head_dept_id
            FROM ""DepartmentHead""
            WHERE ""DepartmentHeadId"" = NEW.""DepartmentHeadId"";

            -- Obtener el DepartmentId del Doctor
            SELECT ""DepartmentId"" INTO v_doctor_dept_id
            FROM ""Doctor""
            WHERE ""EmployeeId"" = NEW.""DoctorId"";

            -- Obtener el DepartmentToId de la Derivation
            SELECT ""DepartmentToId"" INTO v_derivation_dept_to_id
            FROM ""Derivation""
            WHERE ""DerivationId"" = NEW.""DerivationId"";

            -- Validar que el DepartmentHead pertenezca al departamento destino
            IF v_department_head_dept_id <> v_derivation_dept_to_id THEN
                RAISE EXCEPTION 'El jefe de departamento debe pertenecer al mismo departamento destino de la derivación. DepartmentHead.DepartmentId: %, Derivation.DepartmentToId: %', 
                    v_department_head_dept_id, v_derivation_dept_to_id;
            END IF;

            -- Validar que el Doctor pertenezca al departamento destino
            IF v_doctor_dept_id <> v_derivation_dept_to_id THEN
                RAISE EXCEPTION 'El doctor tratante debe pertenecer al mismo departamento destino de la derivación. Doctor.DepartmentId: %, Derivation.DepartmentToId: %', 
                    v_doctor_dept_id, v_derivation_dept_to_id;
            END IF;

            RETURN NEW;
        END;
        $$ LANGUAGE plpgsql;
    ";

    /// <summary>
    /// Script SQL para crear el trigger que ejecuta la validación antes de INSERT o UPDATE
    /// en la tabla ConsultationDerivation.
    /// </summary>
    public static string CreateConsultationDerivationValidationTrigger => @"
        DO $$
        BEGIN
            -- Eliminar el trigger si existe para recrearlo
            DROP TRIGGER IF EXISTS tr_consultation_derivation_validate_department ON ""ConsultationDerivation"";
            
            -- Crear el trigger
            CREATE TRIGGER tr_consultation_derivation_validate_department
            BEFORE INSERT OR UPDATE ON ""ConsultationDerivation""
            FOR EACH ROW
            EXECUTE FUNCTION fn_validate_consultation_derivation_department();
        END $$;
    ";

    /// <summary>
    /// Script SQL para eliminar el trigger de validación de ConsultationDerivation.
    /// </summary>
    public static string DropConsultationDerivationValidationTrigger => @"
        DROP TRIGGER IF EXISTS tr_consultation_derivation_validate_department ON ""ConsultationDerivation"";
    ";

    /// <summary>
    /// Script SQL para eliminar la función de validación de ConsultationDerivation.
    /// </summary>
    public static string DropConsultationDerivationValidationFunction => @"
        DROP FUNCTION IF EXISTS fn_validate_consultation_derivation_department();
    ";

    #endregion
}
