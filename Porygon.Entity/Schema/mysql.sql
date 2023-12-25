CREATE TABLE `poryentity` (
  `Id` char(36) NOT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `Title` varchar(255) DEFAULT NULL,
  `TotalRecords` int NOT NULL DEFAULT '0',
  `State` enum('UNMODIFIED','NEW','UPDATED','DELETED') NOT NULL DEFAULT 'UNMODIFIED',
  `LinkedItemId` char(36) NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000',
  `DateModified` datetime(3) DEFAULT NULL,
  `DateCreated` datetime(3) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
