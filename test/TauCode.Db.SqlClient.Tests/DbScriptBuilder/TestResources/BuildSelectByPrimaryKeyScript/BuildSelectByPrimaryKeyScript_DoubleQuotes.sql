SELECT
    "Height",
    "EnglishDescription",
    "UnicodeDescription",
    "PersonMetaKey",
    "PersonOrdNumber",
    "PersonId"
FROM
    "zeta"."PersonData"
WHERE
    "Id" = @p_id