using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atoms.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGetGamesForUserFunction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION get_games_for_user(visitor_id uuid, user_id text)
                RETURNS TABLE (
                    "Id" uuid,
                    "CreatedDateUtc" timestamptz,
                    "LastUpdatedDateUtc" timestamptz,
                    "Move" integer,
                    "Round" integer,
                    "IsActive" boolean,
                    "Opponents" text,
                    "Winner" text
                ) AS $$
                BEGIN
                    RETURN QUERY
                    SELECT
                        "g"."Id",
                        "g"."CreatedDateUtc",
                        "g"."LastUpdatedDateUtc",
                        "g"."Move",
                        "g"."Round",
                        "g"."IsActive",
                        (
                            SELECT
                                STRING_AGG(
                                    CASE "pt"."Name"
                                        WHEN 'Human' THEN COALESCE("u"."Name", 'Player ' || "p"."Number")
                                        ELSE "pt"."Description"
                                    END, ', ' ORDER BY "p"."Number")
                            FROM
                                "Players" AS "p"
                            INNER JOIN
                                "PlayerTypes" AS "pt" ON "p"."PlayerTypeId" = "pt"."Id"
                            LEFT JOIN
                                "Visitors" AS "u" ON "p"."VisitorId" = "u"."Id"
                            WHERE
                                "p"."GameId" = "g"."Id" AND 
                                ("p"."VisitorId" IS NULL OR "p"."VisitorId" <> visitor_id) AND 
                                (user_id IS NULL OR "p"."UserId" IS NULL OR "p"."UserId" <> user_id)
                        ) AS "Opponents",
                        (
                            SELECT
                                CASE "pt"."Name"
                                    WHEN 'Human' THEN COALESCE("u"."Name", 'Player ' || "p"."Number")
                                    ELSE "pt"."Description"
                                END
                            FROM
                                "Players" AS "p"
                            INNER JOIN
                                "PlayerTypes" AS "pt" ON "p"."PlayerTypeId" = "pt"."Id"
                            LEFT JOIN
                                "Visitors" AS "u" ON "p"."VisitorId" = "u"."Id"
                            WHERE
                                "p"."GameId" = "g"."Id"
                                AND "p"."IsWinner" = TRUE
                        ) AS "Winner"
                    FROM
                        "Games" AS "g"
                    WHERE
                        "g"."VisitorId" = visitor_id OR 
                        "g"."UserId" = user_id OR 
                        EXISTS (
                            SELECT 1
                            FROM "Players" AS "p"
                            WHERE "p"."GameId" = "g"."Id"
                            AND (
                                "p"."VisitorId" = visitor_id OR 
                                "p"."UserId" = user_id)
                        );
                END;
                $$ LANGUAGE plpgsql;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS get_games_for_user(uuid, text);");
        }
    }
}
