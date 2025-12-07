CREATE DATABASE IF NOT EXISTS devtasker;

USE devtasker;

CREATE TABLE IF NOT EXISTS `projects` (
    `Id`          INT           AUTO_INCREMENT NOT NULL,
    `Name`        VARCHAR (50)  NOT NULL,
    `Description` LONGTEXT      NULL,
    `is_archived` TINYINT       DEFAULT ((0)) NOT NULL,
    `created_at`  TIMESTAMP 	DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`Id` ASC)
) AUTO_INCREMENT = 1001;

CREATE TABLE IF NOT EXISTS `task_items` (
    `id`           INT           	AUTO_INCREMENT NOT NULL,
    `project_id`   INT           	NOT NULL,
    `title`        VARCHAR (50)  	NOT NULL,
    `description`  LONGTEXT         NULL,
    `status`       INT           	NOT NULL,
    `priority`     INT           	NOT NULL,
    `due_date`     DATETIME(3)      NULL,
    `completed_at` DATETIME(3)      NULL,
    `created_at`   TIMESTAMP 		DEFAULT CURRENT_TIMESTAMP,
    `is_archived`  TINYINT          DEFAULT ((0)) NOT NULL,
    PRIMARY KEY (`id` ASC),
    CONSTRAINT `FK_task_items_ToProjects` FOREIGN KEY (`project_id`) REFERENCES `projects` (`Id`)
);

CREATE TABLE IF NOT EXISTS `work_logs` (
    `id`           INT           	AUTO_INCREMENT NOT NULL,
    `task_item_id` INT           	NOT NULL,
    `started_at`   DATETIME(3)      NULL,
    `ended_at`     DATETIME(3)      NULL,
    `comment`      LONGTEXT         NULL,
    `created_at`   TIMESTAMP 		DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`id` ASC),
    CONSTRAINT `FK_work_logs_ToTaskItem` FOREIGN KEY (`task_item_id`) REFERENCES `task_items` (`id`)
);
