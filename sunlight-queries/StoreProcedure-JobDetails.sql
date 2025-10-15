CREATE PROCEDURE InsertJobDetails @HeaderId INT AS BEGIN
	INSERT INTO JobDetails (
		JobId,
		DetailId,
		DesignType,
		BlindType,
		Qty,
		FabricType,
		FabricColour,
		TubeSkinSize,
		NumBoldNuts,
		MotorStyle,
		MotorRemote,
		MotorCharger,
		[Trim],
		RollDirection,
		HardwareType,
		HardwareColour,
		ControlPosition,
		ChainLength,
		ChainColour,
		ChildSafe,
		ControlType,
		ControlLength,
		TubeSize,
		BrailType,
		BrailColour,
		Accessories,
		SpringAssist,
		BracketType,
		LinkBlinds, 
		BracketColour,
		--|| Alumminium||
		BracketOption,
		WandLength,
		CutOutLeftTop,
		CutOutRightTop,
		CutOutLeftBot,
		CutOutRightBot,
		LHSWidthTop,
		LHSHeightTop,
		RHSWidthTop,
		RHSHeightTop,
		LHSWidthBot,
		LHSHeightBot,
		RHSWidthBot,
		RHSHeightBot,
		BottomHoldDown,
		--||Vertical||
		TrackColour,
		SlatSize,
		CarriesQty,
		SpacerSize,
		TrackOption,
		StackPosition,
		HangerType,
		BottomColour,
		LeftHandReturn,
		RightHandReturn,
		Sloper,
		InsertInTrack,
		FabricCutDrop,
	
		Width,
		[Drop],
		SpringType,
		Mounting,
		Room
	) 
	SELECT
	Jobs.Id AS JobId,
	view_details.Id AS DetailId,
	view_details.DesignName AS DesignType,
	view_details.BlindName AS BlindType,
	view_details.Qty,
	view_details.FabricType,
	view_details.FabricColour,
	--||---------------------------TubeSkinSize-------------------------------------||
	CASE 
		WHEN view_details.TubeSize = 40 THEN view_details.Width - 28
		WHEN view_details.TubeSize IN (45, '45H') THEN view_details.Width - 32
		ELSE 0
	END AS TubeSkinSize,
	--||----------------------------NumBoldNuts-------------------------------------||
	CASE 
		WHEN view_details.BottomType = 'Oval' OR view_details.BottomType = 'Round' OR view_details.BottomType LIKE '%Flat%' THEN 
			CASE 
	      WHEN view_details.[Trim] = '1F' THEN
					CASE
	          WHEN view_details.TubeSize = 40 THEN view_details.[Drop] + 200
	          WHEN view_details.TubeSize IN (43,45,'45h',50) THEN view_details.[Drop] + 300
						WHEN view_details.TubeSize = 60 THEN view_details.[Drop] + 350
	          ELSE 0
            END
	      ELSE 0
      END
		WHEN view_details.[Trim] <> '1F' THEN
			CASE
	      WHEN view_details.TubeSize = 40 THEN view_details.[Drop] + 250
	      WHEN view_details.TubeSize IN (43,45,'45h',50) THEN view_details.[Drop] + 350
				WHEN view_details.TubeSize = 60 THEN view_details.[Drop] + 400
	      ELSE 0
      END
		ELSE 0
	END AS NumBoldNuts,
	view_details.MotorStyle,
	view_details.MotorRemote,
	view_details.MotorCharger,
	view_details.[Trim],
	view_details.RollDirection,
	view_details.TubeType AS HardwareType,
	view_details.ColourType HardwareColour,
	view_details.ControlPosition,
	view_details.ChainLength,
	view_details.ChainColour,
	view_details.ChildSafe,
	view_details.ControlType,
	view_details.ControlLength,
	view_details.TubeSize,
	view_details.BottomType AS BrailType,
	view_details.BottomColour AS BrailColour,
	view_details.Accessory AS Accessories,
	'?' AS SpringAssist,
	view_details.BracketType,
	view_details.BlindNo AS LinkBlinds,
	view_details.BracketColour,
	--|| Alumminium||
	view_details.BracketOption,
	view_details.WandLength,
	view_details.CutOut_LeftTop As CutOutLeftTop,
	view_details.CutOut_RightTop AS CutOutRightTop,
	view_details.CutOut_LeftBottom AS CutOutLeftBot,
	view_details.CutOut_RightBottom AS CutOutRightBot,
	view_details.LHSWidth_Top AS LHSWidthTop,
	view_details.LHSHeight_Top AS LHSHeightTop,
	view_details.RHSWidth_Top AS RHSWidthTop,
	view_details.RHSHeight_Top AS RHSHeightTop,
	view_details.LHSWidth_Bottom AS LHSWidthBot,
	view_details.LHSHeight_Bottom AS LHSHeightBot,
	view_details.RHSWidth_Bottom AS RHSWidthBot,
	view_details.RHSHeight_Bottom AS RHSHeightBot,
	view_details.BottomHoldDown,
	--||Vertical||
	view_details.TrackColour,
	view_details.SlatSize,
	'0' AS CarriesQty,
	'0' AS SpacerSize,
	'?' AS TrackOption,
	view_details.StackPosition,
	view_details.HangerType,
	'?' AS BottomColour,
	'0' AS LeftHandReturn,
	'0' AS RightHandReturn,
	view_details.Sloper,
	view_details.InsertInTrack,
	'0' AS FabricCutDrop,
	view_details.Width,
	view_details.[Drop],
	'?' AS SpringType,
	view_details.Mounting,
	view_details.Location  AS Room
	FROM
		view_details
		INNER JOIN Jobs ON Jobs.HeaderId= view_details.HeaderId
		CROSS JOIN ( SELECT TOP 1 Id FROM Jobs WHERE HeaderId = @HeaderId ORDER BY Id DESC ) AS LastJob 
	WHERE
		view_details.HeaderId= @HeaderId 
		AND Jobs.Id = LastJob.Id 
		AND view_details.Active=1
	END;