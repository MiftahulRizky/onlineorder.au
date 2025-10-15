ALTER PROCEDURE CreateJobSheets
    @HeaderId INT
AS
BEGIN
   WITH RangkedData AS (
	SELECT
		JobDetails.JobId,
		Jobs.JoNumber,
		Jobs.HeaderId,
		JobDetails.DesignType,
		JobDetails.BlindType,
		view_headers.StoreName,
		view_headers.OrderNo,
		view_headers.OrderCust,
		view_headers.Delivery AS ZoneId,
		view_headers.UserName,
		view_headers.CreatedDate AS OrderCreated,
		Jobs.CreatedDate AS JobCreated,
		view_headers.SubmittedDate AS ShipDate,
		JobDetails.Qty,
		JobDetails.FabricType,
		JobDetails.FabricColour,
		JobDetails.TubeSkinSize,
		JobDetails.NumBoldNuts,
    JobDetails.MotorStyle,
		JobDetails.MotorRemote,
		JobDetails.MotorCharger,
		JobDetails.[Trim] AS Trims,
		JobDetails.RollDirection,
		JobDetails.HardwareType,
		JobDetails.HardwareColour,
		JobDetails.ControlPosition,
		JobDetails.ChainLength,
		JobDetails.ChainColour,
		JobDetails.ChildSafe,
		JobDetails.ControlType,
		JobDetails.ControlLength,
		JobDetails.TubeSize,
		JobDetails.BrailType,
		JobDetails.BrailColour,
		JobDetails.Accessories,
		JobDetails.SpringAssist,
		JobDetails.BracketType,
		JobDetails.LinkBlinds,
		JobDetails.BracketColour,
		--|| Alumminium||
		JobDetails.BracketOption,
		JobDetails.WandLength,
		JobDetails.CutOutLeftTop,
		JobDetails.CutOutRightTop,
		JobDetails.CutOutLeftBot,
		JobDetails.CutOutRightBot,
		JobDetails.LHSWidthTop,
		JobDetails.LHSHeightTop,
		JobDetails.RHSWidthTop,
		JobDetails.RHSHeightTop,
		JobDetails.LHSWidthBot,
		JobDetails.LHSHeightBot,
		JobDetails.RHSWidthBot,
		JobDetails.RHSHeightBot,
		JobDetails.BottomHoldDown,
		--||Vertical||
		JobDetails.TrackColour,
		JobDetails.SlatSize,
		JobDetails.CarriesQty,
		JobDetails.SpacerSize,
		JobDetails.TrackOption,
		JobDetails.StackPosition,
		JobDetails.HangerType,
		JobDetails.BottomColour,
		JobDetails.LeftHandReturn,
		JobDetails.RightHandReturn,
		JobDetails.Sloper,
		JobDetails.InsertInTrack,
		JobDetails.FabricCutDrop,
		
		JobDetails.Width,
		JobDetails.[Drop] AS Drops,
		JobDetails.SpringType,
		JobDetails.Mounting,
		JobDetails.Room,
		ROW_NUMBER() OVER (PARTITION BY JobDetails.BlindType ORDER BY JobDetails.DetailId) AS maxData 
	FROM
		JobDetails
		INNER JOIN Jobs ON Jobs.Id = JobDetails.JobId
		INNER JOIN view_headers ON view_headers.Id = Jobs.HeaderId 
		CROSS JOIN (SELECT TOP 1 Id FROM Jobs WHERE HeaderId= @HeaderId  ORDER BY Id DESC) AS Params
-- 		CROSS JOIN Params
	WHERE
		Jobs.Id=Params.Id
),
GroupData AS (
	SELECT
		--Field
		JobId,
		JoNumber,
		HeaderId,
		DesignType,
		BlindType,
		StoreName,
		OrderNo,
		OrderCust,
		ZoneId,
		UserName,
		OrderCreated,
		JobCreated,
		ShipDate,
		Qty,
		FabricType,
		FabricColour,
		TubeSkinSize,
		NumBoldNuts,
    	MotorStyle,
		MotorRemote,
		MotorCharger,
		Trims,
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
		Drops,
		SpringType,
		Mounting,
		Room,
		--Filed Max
		maxData,
		(maxData - 1) / 6 AS amountMax,
		SUM(Qty) OVER(PARTITION BY BlindType) AS AmountBlinds
	FROM
		RangkedData 
) 
	SELECT 
		MAX(CASE WHEN maxData % 6 = 1 THEN JobId END) AS JobId,
		CASE WHEN MAX(CASE WHEN maxData % 6 = 1 THEN BlindType END) IS NOT NULL THEN  CAST(1 + (amountMax * 1) AS VARCHAR) END AS pageOf,
		COUNT(BlindType) as AmountOfPage,
		MAX(CASE WHEN maxData % 6 = 1 THEN JoNumber END) AS JoNumber,
		MAX(CASE WHEN maxData % 6 = 1 THEN HeaderId END) AS HeaderId,
		MAX(CASE WHEN maxData % 6 = 1 THEN DesignType END) AS DesignType,
		BlindType,
		MAX(AmountBlinds) AS AmountBlinds,
		MAX(CASE WHEN maxData % 6 = 1 THEN StoreName END) AS StoreName,
		MAX(CASE WHEN maxData % 6 = 1 THEN OrderNo END) AS OrderNo,
		MAX(CASE WHEN maxData % 6 = 1 THEN OrderCust END) AS OrderCust,
		MAX(CASE WHEN maxData % 6 = 1 THEN ZoneId END) AS ZoneId,
		MAX(CASE WHEN maxData % 6 = 1 THEN UserName END) AS UserName,
		MAX(CASE WHEN maxData % 6 = 1 THEN OrderCreated END) AS OrderCreated,
		MAX(CASE WHEN maxData % 6 = 1 THEN JobCreated END) AS JobCreated,
		MAX(CASE WHEN maxData % 6 = 1 THEN ShipDate END) AS ShipDate,

		-- Generate Line fields based on the presence of Qty values, continuing from previous batch
		CASE WHEN MAX(CASE WHEN maxData % 6 = 1 THEN Qty END) IS NOT NULL THEN 'Line ' + CAST(1 + (amountMax * 6) AS VARCHAR) END AS Line1,
		CASE WHEN MAX(CASE WHEN maxData % 6 = 2 THEN Qty END) IS NOT NULL THEN 'Line ' + CAST(2 + (amountMax * 6) AS VARCHAR) END AS Line2,
		CASE WHEN MAX(CASE WHEN maxData % 6 = 3 THEN Qty END) IS NOT NULL THEN 'Line ' + CAST(3 + (amountMax * 6) AS VARCHAR) END AS Line3,
		CASE WHEN MAX(CASE WHEN maxData % 6 = 4 THEN Qty END) IS NOT NULL THEN 'Line ' + CAST(4 + (amountMax * 6) AS VARCHAR) END AS Line4,
		CASE WHEN MAX(CASE WHEN maxData % 6 = 5 THEN Qty END) IS NOT NULL THEN 'Line ' + CAST(5 + (amountMax * 6) AS VARCHAR) END AS Line5,
		CASE WHEN MAX(CASE WHEN maxData % 6 = 0 THEN Qty END) IS NOT NULL THEN 'Line ' + CAST(6 + (amountMax * 6) AS VARCHAR) END AS Line6,
		
			--|| Qty berdasarkan setiap kondisi ||
		MAX(CASE WHEN maxData % 6 = 1 THEN Qty END) AS Qty1,
		MAX(CASE WHEN maxData % 6 = 2 THEN Qty END) AS Qty2,
		MAX(CASE WHEN maxData % 6 = 3 THEN Qty END) AS Qty3,
		MAX(CASE WHEN maxData % 6 = 4 THEN Qty END) AS Qty4,
		MAX(CASE WHEN maxData % 6 = 5 THEN Qty END) AS Qty5,
		MAX(CASE WHEN maxData % 6 = 0 THEN Qty END) AS Qty6,
		
		--|| VenType berdasarkan setiap kondisi ||
		MAX(CASE WHEN maxData % 6 = 1 THEN BlindType END) AS VenType1,
		MAX(CASE WHEN maxData % 6 = 2 THEN BlindType END) AS VenType2,
		MAX(CASE WHEN maxData % 6 = 3 THEN BlindType END) AS VenType3,
		MAX(CASE WHEN maxData % 6 = 4 THEN BlindType END) AS VenType4,
		MAX(CASE WHEN maxData % 6 = 5 THEN BlindType END) AS VenType5,
		MAX(CASE WHEN maxData % 6 = 0 THEN BlindType END) AS VenType6,

		--|| FabricType ||
		MAX(CASE WHEN maxData % 6 = 1 THEN FabricType END) AS FabricType1,
		MAX(CASE WHEN maxData % 6 = 2 THEN FabricType END) AS FabricType2,
		MAX(CASE WHEN maxData % 6 = 3 THEN FabricType END) AS FabricType3,
		MAX(CASE WHEN maxData % 6 = 4 THEN FabricType END) AS FabricType4,
		MAX(CASE WHEN maxData % 6 = 5 THEN FabricType END) AS FabricType5,
		MAX(CASE WHEN maxData % 6 = 0 THEN FabricType END) AS FabricType6,

		--|| FabricColour ||
		MAX(CASE WHEN maxData % 6 = 1 THEN FabricColour END) AS FabricColour1,
		MAX(CASE WHEN maxData % 6 = 2 THEN FabricColour END) AS FabricColour2,
		MAX(CASE WHEN maxData % 6 = 3 THEN FabricColour END) AS FabricColour3,
		MAX(CASE WHEN maxData % 6 = 4 THEN FabricColour END) AS FabricColour4,
		MAX(CASE WHEN maxData % 6 = 5 THEN FabricColour END) AS FabricColour5,
		MAX(CASE WHEN maxData % 6 = 0 THEN FabricColour END) AS FabricColour6,
		
		--|| TubeSkinSize Or TubeSkinSize ||
		MAX(CASE WHEN maxData % 6 = 1 THEN TubeSkinSize END) AS TubeSkinSize1,
		MAX(CASE WHEN maxData % 6 = 2 THEN TubeSkinSize END) AS TubeSkinSize2,
		MAX(CASE WHEN maxData % 6 = 3 THEN TubeSkinSize END) AS TubeSkinSize3,
		MAX(CASE WHEN maxData % 6 = 4 THEN TubeSkinSize END) AS TubeSkinSize4,
		MAX(CASE WHEN maxData % 6 = 5 THEN TubeSkinSize END) AS TubeSkinSize5,
		MAX(CASE WHEN maxData % 6 = 0 THEN TubeSkinSize END) AS TubeSkinSize6,
		
		--|| NumboldNuts Or SkinDrop ||
		MAX(CASE WHEN maxData % 6 = 1 THEN NumBoldNuts END) AS NumBoldNuts1,
		MAX(CASE WHEN maxData % 6 = 2 THEN NumBoldNuts END) AS NumBoldNuts2,
		MAX(CASE WHEN maxData % 6 = 3 THEN NumBoldNuts END) AS NumBoldNuts3,
		MAX(CASE WHEN maxData % 6 = 4 THEN NumBoldNuts END) AS NumBoldNuts4,
		MAX(CASE WHEN maxData % 6 = 5 THEN NumBoldNuts END) AS NumBoldNuts5,
		MAX(CASE WHEN maxData % 6 = 0 THEN NumBoldNuts END) AS NumBoldNuts6,

        --|| MotorStyle ||
		MAX(CASE WHEN maxData % 6 = 1 THEN MotorStyle END) AS MotorStyle1,
		MAX(CASE WHEN maxData % 6 = 2 THEN MotorStyle END) AS MotorStyle2,
		MAX(CASE WHEN maxData % 6 = 3 THEN MotorStyle END) AS MotorStyle3,
		MAX(CASE WHEN maxData % 6 = 4 THEN MotorStyle END) AS MotorStyle4,
		MAX(CASE WHEN maxData % 6 = 5 THEN MotorStyle END) AS MotorStyle5,
		MAX(CASE WHEN maxData % 6 = 0 THEN MotorStyle END) AS MotorStyle6,

        --|| MotorRemote ||
		MAX(CASE WHEN maxData % 6 = 1 THEN MotorRemote END) AS MotorRemote1,
		MAX(CASE WHEN maxData % 6 = 2 THEN MotorRemote END) AS MotorRemote2,
		MAX(CASE WHEN maxData % 6 = 3 THEN MotorRemote END) AS MotorRemote3,
		MAX(CASE WHEN maxData % 6 = 4 THEN MotorRemote END) AS MotorRemote4,
		MAX(CASE WHEN maxData % 6 = 5 THEN MotorRemote END) AS MotorRemote5,
		MAX(CASE WHEN maxData % 6 = 0 THEN MotorRemote END) AS MotorRemote6,

        --|| MotorCharger ||
		MAX(CASE WHEN maxData % 6 = 1 THEN MotorCharger END) AS MotorCharger1,
		MAX(CASE WHEN maxData % 6 = 2 THEN MotorCharger END) AS MotorCharger2,
		MAX(CASE WHEN maxData % 6 = 3 THEN MotorCharger END) AS MotorCharger3,
		MAX(CASE WHEN maxData % 6 = 4 THEN MotorCharger END) AS MotorCharger4,
		MAX(CASE WHEN maxData % 6 = 5 THEN MotorCharger END) AS MotorCharger5,
		MAX(CASE WHEN maxData % 6 = 0 THEN MotorCharger END) AS MotorCharger6,
		
		--|| Trim Or Trims ||
		MAX(CASE WHEN maxData % 6 = 1 THEN Trims END) AS Trims1,
		MAX(CASE WHEN maxData % 6 = 2 THEN Trims END) AS Trims2,
		MAX(CASE WHEN maxData % 6 = 3 THEN Trims END) AS Trims3,
		MAX(CASE WHEN maxData % 6 = 4 THEN Trims END) AS Trims4,
		MAX(CASE WHEN maxData % 6 = 5 THEN Trims END) AS Trims5,
		MAX(CASE WHEN maxData % 6 = 0 THEN Trims END) AS Trims6,
		
		--|| Roll Direction ||
		MAX(CASE WHEN maxData % 6 = 1 THEN RollDirection END) AS RollDirection1,
		MAX(CASE WHEN maxData % 6 = 2 THEN RollDirection END) AS RollDirection2,
		MAX(CASE WHEN maxData % 6 = 3 THEN RollDirection END) AS RollDirection3,
		MAX(CASE WHEN maxData % 6 = 4 THEN RollDirection END) AS RollDirection4,
		MAX(CASE WHEN maxData % 6 = 5 THEN RollDirection END) AS RollDirection5,
		MAX(CASE WHEN maxData % 6 = 0 THEN RollDirection END) AS RollDirection6,
		
		--|| Trim Or Trims ||
		MAX(CASE WHEN maxData % 6 = 1 THEN HardwareType END) AS HardwareType1,
		MAX(CASE WHEN maxData % 6 = 2 THEN HardwareType END) AS HardwareType2,
		MAX(CASE WHEN maxData % 6 = 3 THEN HardwareType END) AS HardwareType3,
		MAX(CASE WHEN maxData % 6 = 4 THEN HardwareType END) AS HardwareType4,
		MAX(CASE WHEN maxData % 6 = 5 THEN HardwareType END) AS HardwareType5,
		MAX(CASE WHEN maxData % 6 = 0 THEN HardwareType END) AS HardwareType6,
		
		--|| Trim Or Trims ||
		MAX(CASE WHEN maxData % 6 = 1 THEN HardwareColour END) AS HardwareColour1,
		MAX(CASE WHEN maxData % 6 = 2 THEN HardwareColour END) AS HardwareColour2,
		MAX(CASE WHEN maxData % 6 = 3 THEN HardwareColour END) AS HardwareColour3,
		MAX(CASE WHEN maxData % 6 = 4 THEN HardwareColour END) AS HardwareColour4,
		MAX(CASE WHEN maxData % 6 = 5 THEN HardwareColour END) AS HardwareColour5,
		MAX(CASE WHEN maxData % 6 = 0 THEN HardwareColour END) AS HardwareColour6,
		
		--|| ControlPosition Or ControlPosition ||
		MAX(CASE WHEN maxData % 6 = 1 THEN ControlPosition END) AS ControlPosition1,
		MAX(CASE WHEN maxData % 6 = 2 THEN ControlPosition END) AS ControlPosition2,
		MAX(CASE WHEN maxData % 6 = 3 THEN ControlPosition END) AS ControlPosition3,
		MAX(CASE WHEN maxData % 6 = 4 THEN ControlPosition END) AS ControlPosition4,
		MAX(CASE WHEN maxData % 6 = 5 THEN ControlPosition END) AS ControlPosition5,
		MAX(CASE WHEN maxData % 6 = 0 THEN ControlPosition END) AS ControlPosition6,
		
		--|| ChainColour ||
		MAX(CASE WHEN maxData % 6 = 1 THEN ChainColour END) AS ChainColour1,
		MAX(CASE WHEN maxData % 6 = 2 THEN ChainColour END) AS ChainColour2,
		MAX(CASE WHEN maxData % 6 = 3 THEN ChainColour END) AS ChainColour3,
		MAX(CASE WHEN maxData % 6 = 4 THEN ChainColour END) AS ChainColour4,
		MAX(CASE WHEN maxData % 6 = 5 THEN ChainColour END) AS ChainColour5,
		MAX(CASE WHEN maxData % 6 = 0 THEN ChainColour END) AS ChainColour6,
		
		--|| ChildSafe||
		MAX(CASE WHEN maxData % 6 = 1 THEN ChildSafe END) AS ChildSafe1,
		MAX(CASE WHEN maxData % 6 = 2 THEN ChildSafe END) AS ChildSafe2,
		MAX(CASE WHEN maxData % 6 = 3 THEN ChildSafe END) AS ChildSafe3,
		MAX(CASE WHEN maxData % 6 = 4 THEN ChildSafe END) AS ChildSafe4,
		MAX(CASE WHEN maxData % 6 = 5 THEN ChildSafe END) AS ChildSafe5,
		MAX(CASE WHEN maxData % 6 = 0 THEN ChildSafe END) AS ChildSafe6,
		
		--|| ControlType||
		MAX(CASE WHEN maxData % 6 = 1 THEN ControlType END) AS ControlType1,
		MAX(CASE WHEN maxData % 6 = 2 THEN ControlType END) AS ControlType2,
		MAX(CASE WHEN maxData % 6 = 3 THEN ControlType END) AS ControlType3,
		MAX(CASE WHEN maxData % 6 = 4 THEN ControlType END) AS ControlType4,
		MAX(CASE WHEN maxData % 6 = 5 THEN ControlType END) AS ControlType5,
		MAX(CASE WHEN maxData % 6 = 0 THEN ControlType END) AS ControlType6,
		
		--|| ControlLength Or ControlLength ||
		MAX(CASE WHEN maxData % 6 = 1 THEN ControlLength END) AS ControlLength1,
		MAX(CASE WHEN maxData % 6 = 2 THEN ControlLength END) AS ControlLength2,
		MAX(CASE WHEN maxData % 6 = 3 THEN ControlLength END) AS ControlLength3,
		MAX(CASE WHEN maxData % 6 = 4 THEN ControlLength END) AS ControlLength4,
		MAX(CASE WHEN maxData % 6 = 5 THEN ControlLength END) AS ControlLength5,
		MAX(CASE WHEN maxData % 6 = 0 THEN ControlLength END) AS ControlLength6,
		
		--|| TubeSize Or TubeSize ||
		MAX(CASE WHEN maxData % 6 = 1 THEN TubeSize END) AS TubeSize1,
		MAX(CASE WHEN maxData % 6 = 2 THEN TubeSize END) AS TubeSize2,
		MAX(CASE WHEN maxData % 6 = 3 THEN TubeSize END) AS TubeSize3,
		MAX(CASE WHEN maxData % 6 = 4 THEN TubeSize END) AS TubeSize4,
		MAX(CASE WHEN maxData % 6 = 5 THEN TubeSize END) AS TubeSize5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN TubeSize END) AS TubeSize6,
		
		--|| BrailType Or BrailType ||
		MAX(CASE WHEN maxData % 6 = 1 THEN BrailType END) AS BrailType1,
		MAX(CASE WHEN maxData % 6 = 2 THEN BrailType END) AS BrailType2,
		MAX(CASE WHEN maxData % 6 = 3 THEN BrailType END) AS BrailType3,
		MAX(CASE WHEN maxData % 6 = 4 THEN BrailType END) AS BrailType4,
		MAX(CASE WHEN maxData % 6 = 5 THEN BrailType END) AS BrailType5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN BrailType END) AS BrailType6,
		
		--|| BrailColour Or BrailColour ||
		MAX(CASE WHEN maxData % 6 = 1 THEN BrailColour END) AS BrailColour1,
		MAX(CASE WHEN maxData % 6 = 2 THEN BrailColour END) AS BrailColour2,
		MAX(CASE WHEN maxData % 6 = 3 THEN BrailColour END) AS BrailColour3,
		MAX(CASE WHEN maxData % 6 = 4 THEN BrailColour END) AS BrailColour4,
		MAX(CASE WHEN maxData % 6 = 5 THEN BrailColour END) AS BrailColour5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN BrailColour END) AS BrailColour6,
		
		--|| Accessories Or Accessories ||
		MAX(CASE WHEN maxData % 6 = 1 THEN Accessories END) AS Accessories1,
		MAX(CASE WHEN maxData % 6 = 2 THEN Accessories END) AS Accessories2,
		MAX(CASE WHEN maxData % 6 = 3 THEN Accessories END) AS Accessories3,
		MAX(CASE WHEN maxData % 6 = 4 THEN Accessories END) AS Accessories4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN Accessories END) AS Accessories5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN Accessories END) AS Accessories6,
		
		--|| SpringAssist Or SpringAssist ||
		MAX(CASE WHEN maxData % 6 = 1 THEN SpringAssist END) AS SpringAssist1,
		MAX(CASE WHEN maxData % 6 = 2 THEN SpringAssist END) AS SpringAssist2,
		MAX(CASE WHEN maxData % 6 = 3 THEN SpringAssist END) AS SpringAssist3,
		MAX(CASE WHEN maxData % 6 = 4 THEN SpringAssist END) AS SpringAssist4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN SpringAssist END) AS SpringAssist5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN SpringAssist END) AS SpringAssist6,
		
		--|| BracketType Or BracketType ||
		MAX(CASE WHEN maxData % 6 = 1 THEN BracketType END) AS BracketType1,
		MAX(CASE WHEN maxData % 6 = 2 THEN BracketType END) AS BracketType2,
		MAX(CASE WHEN maxData % 6 = 3 THEN BracketType END) AS BracketType3,
		MAX(CASE WHEN maxData % 6 = 4 THEN BracketType END) AS BracketType4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN BracketType END) AS BracketType5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN BracketType END) AS BracketType6,
		
		--|| LinkBlinds Or LinkBlinds ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN LinkBlinds END) AS LinkBlinds1,
		MAX(CASE WHEN maxData % 6 = 2 THEN LinkBlinds END) AS LinkBlinds2,
		MAX(CASE WHEN maxData % 6 = 3 THEN LinkBlinds END) AS LinkBlinds3,
		MAX(CASE WHEN maxData % 6 = 4 THEN LinkBlinds END) AS LinkBlinds4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN LinkBlinds END) AS LinkBlinds5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN LinkBlinds END) AS LinkBlinds6,
		
		--|| BracketColour Or BracketColour ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN BracketColour END) AS BracketColour1,
		MAX(CASE WHEN maxData % 6 = 2 THEN BracketColour END) AS BracketColour2,
		MAX(CASE WHEN maxData % 6 = 3 THEN BracketColour END) AS BracketColour3,
		MAX(CASE WHEN maxData % 6 = 4 THEN BracketColour END) AS BracketColour4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN BracketColour END) AS BracketColour5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN BracketColour END) AS BracketColour6,

		--|| WandLength Or WandLength ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN WandLength END) AS WandLength1,
		MAX(CASE WHEN maxData % 6 = 2 THEN WandLength END) AS WandLength2,
		MAX(CASE WHEN maxData % 6 = 3 THEN WandLength END) AS WandLength3,
		MAX(CASE WHEN maxData % 6 = 4 THEN WandLength END) AS WandLength4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN WandLength END) AS WandLength5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN WandLength END) AS WandLength6,
		
		--|| BracketOption Or BracketOption ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN BracketOption END) AS BracketOption1,
		MAX(CASE WHEN maxData % 6 = 2 THEN BracketOption END) AS BracketOption2,
		MAX(CASE WHEN maxData % 6 = 3 THEN BracketOption END) AS BracketOption3,
		MAX(CASE WHEN maxData % 6 = 4 THEN BracketOption END) AS BracketOption4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN BracketOption END) AS BracketOption5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN BracketOption END) AS BracketOption6,
		
		--|| TrackColour Or TrackColour ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN TrackColour END) AS TrackColour1,
		MAX(CASE WHEN maxData % 6 = 2 THEN TrackColour END) AS TrackColour2,
		MAX(CASE WHEN maxData % 6 = 3 THEN TrackColour END) AS TrackColour3,
		MAX(CASE WHEN maxData % 6 = 4 THEN TrackColour END) AS TrackColour4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN TrackColour END) AS TrackColour5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN TrackColour END) AS TrackColour6,

		--|| SlatSize Or SlatSize ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN SlatSize END) AS SlatSize1,
		MAX(CASE WHEN maxData % 6 = 2 THEN SlatSize END) AS SlatSize2,
		MAX(CASE WHEN maxData % 6 = 3 THEN SlatSize END) AS SlatSize3,
		MAX(CASE WHEN maxData % 6 = 4 THEN SlatSize END) AS SlatSize4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN SlatSize END) AS SlatSize5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN SlatSize END) AS SlatSize6,

		--|| CarriesQty Or CarriesQty ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN CarriesQty END) AS CarriesQty1,
		MAX(CASE WHEN maxData % 6 = 2 THEN CarriesQty END) AS CarriesQty2,
		MAX(CASE WHEN maxData % 6 = 3 THEN CarriesQty END) AS CarriesQty3,
		MAX(CASE WHEN maxData % 6 = 4 THEN CarriesQty END) AS CarriesQty4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN CarriesQty END) AS CarriesQty5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN CarriesQty END) AS CarriesQty6,

		--|| SpacerSize Or SpacerSize ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN SpacerSize END) AS SpacerSize1,
		MAX(CASE WHEN maxData % 6 = 2 THEN SpacerSize END) AS SpacerSize2,
		MAX(CASE WHEN maxData % 6 = 3 THEN SpacerSize END) AS SpacerSize3,
		MAX(CASE WHEN maxData % 6 = 4 THEN SpacerSize END) AS SpacerSize4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN SpacerSize END) AS SpacerSize5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN SpacerSize END) AS SpacerSize6,
		
		--|| TrackOption Or TrackOption ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN TrackOption END) AS TrackOption1,
		MAX(CASE WHEN maxData % 6 = 2 THEN TrackOption END) AS TrackOption2,
		MAX(CASE WHEN maxData % 6 = 3 THEN TrackOption END) AS TrackOption3,
		MAX(CASE WHEN maxData % 6 = 4 THEN TrackOption END) AS TrackOption4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN TrackOption END) AS TrackOption5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN TrackOption END) AS TrackOption6,

		--|| StackPosition Or StackPosition ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN StackPosition END) AS StackPosition1,
		MAX(CASE WHEN maxData % 6 = 2 THEN StackPosition END) AS StackPosition2,
		MAX(CASE WHEN maxData % 6 = 3 THEN StackPosition END) AS StackPosition3,
		MAX(CASE WHEN maxData % 6 = 4 THEN StackPosition END) AS StackPosition4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN StackPosition END) AS StackPosition5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN StackPosition END) AS StackPosition6,

		--|| ChainLength Or ChainLength ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN ChainLength END) AS ChainLength1,
		MAX(CASE WHEN maxData % 6 = 2 THEN ChainLength END) AS ChainLength2,
		MAX(CASE WHEN maxData % 6 = 3 THEN ChainLength END) AS ChainLength3,
		MAX(CASE WHEN maxData % 6 = 4 THEN ChainLength END) AS ChainLength4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN ChainLength END) AS ChainLength5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN ChainLength END) AS ChainLength6,

		--|| HangerType Or HangerType ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN HangerType END) AS HangerType1,
		MAX(CASE WHEN maxData % 6 = 2 THEN HangerType END) AS HangerType2,
		MAX(CASE WHEN maxData % 6 = 3 THEN HangerType END) AS HangerType3,
		MAX(CASE WHEN maxData % 6 = 4 THEN HangerType END) AS HangerType4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN HangerType END) AS HangerType5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN HangerType END) AS HangerType6,

		--|| BottomHoldDown Or BottomHoldDown ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN BottomHoldDown END) AS BottomHoldDown1,
		MAX(CASE WHEN maxData % 6 = 2 THEN BottomHoldDown END) AS BottomHoldDown2,
		MAX(CASE WHEN maxData % 6 = 3 THEN BottomHoldDown END) AS BottomHoldDown3,
		MAX(CASE WHEN maxData % 6 = 4 THEN BottomHoldDown END) AS BottomHoldDown4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN BottomHoldDown END) AS BottomHoldDown5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN BottomHoldDown END) AS BottomHoldDown6,


		--|| BottomColour Or BottomColour ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN BottomColour END) AS BottomColour1,
		MAX(CASE WHEN maxData % 6 = 2 THEN BottomColour END) AS BottomColour2,
		MAX(CASE WHEN maxData % 6 = 3 THEN BottomColour END) AS BottomColour3,
		MAX(CASE WHEN maxData % 6 = 4 THEN BottomColour END) AS BottomColour4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN BottomColour END) AS BottomColour5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN BottomColour END) AS BottomColour6,

		--|| LeftHandReturn Or LeftHandReturn ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN LeftHandReturn END) AS LeftHandReturn1,
		MAX(CASE WHEN maxData % 6 = 2 THEN LeftHandReturn END) AS LeftHandReturn2,
		MAX(CASE WHEN maxData % 6 = 3 THEN LeftHandReturn END) AS LeftHandReturn3,
		MAX(CASE WHEN maxData % 6 = 4 THEN LeftHandReturn END) AS LeftHandReturn4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN LeftHandReturn END) AS LeftHandReturn5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN LeftHandReturn END) AS LeftHandReturn6,

		--|| RightHandReturn Or RightHandReturn ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN RightHandReturn END) AS RightHandReturn1,
		MAX(CASE WHEN maxData % 6 = 2 THEN RightHandReturn END) AS RightHandReturn2,
		MAX(CASE WHEN maxData % 6 = 3 THEN RightHandReturn END) AS RightHandReturn3,
		MAX(CASE WHEN maxData % 6 = 4 THEN RightHandReturn END) AS RightHandReturn4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN RightHandReturn END) AS RightHandReturn5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN RightHandReturn END) AS RightHandReturn6,
		
		--|| Sloper Or Sloper ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN Sloper END) AS Sloper1,
		MAX(CASE WHEN maxData % 6 = 2 THEN Sloper END) AS Sloper2,
		MAX(CASE WHEN maxData % 6 = 3 THEN Sloper END) AS Sloper3,
		MAX(CASE WHEN maxData % 6 = 4 THEN Sloper END) AS Sloper4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN Sloper END) AS Sloper5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN Sloper END) AS Sloper6,

		--|| InsertInTrack Or InsertInTrack ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN InsertInTrack END) AS InsertInTrack1,
		MAX(CASE WHEN maxData % 6 = 2 THEN InsertInTrack END) AS InsertInTrack2,
		MAX(CASE WHEN maxData % 6 = 3 THEN InsertInTrack END) AS InsertInTrack3,
		MAX(CASE WHEN maxData % 6 = 4 THEN InsertInTrack END) AS InsertInTrack4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN InsertInTrack END) AS InsertInTrack5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN InsertInTrack END) AS InsertInTrack6,

		--|| FabricCutDrop Or FabricCutDrop ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN FabricCutDrop END) AS FabricCutDrop1,
		MAX(CASE WHEN maxData % 6 = 2 THEN FabricCutDrop END) AS FabricCutDrop2,
		MAX(CASE WHEN maxData % 6 = 3 THEN FabricCutDrop END) AS FabricCutDrop3,
		MAX(CASE WHEN maxData % 6 = 4 THEN FabricCutDrop END) AS FabricCutDrop4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN FabricCutDrop END) AS FabricCutDrop5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN FabricCutDrop END) AS FabricCutDrop6,
		
		--|| Width Or Width ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN Width END) AS Width1,
		MAX(CASE WHEN maxData % 6 = 2 THEN Width END) AS Width2,
		MAX(CASE WHEN maxData % 6 = 3 THEN Width END) AS Width3,
		MAX(CASE WHEN maxData % 6 = 4 THEN Width END) AS Width4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN Width END) AS Width5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN Width END) AS Width6,
		
		--|| Drops Or Drops ||	
		MAX(CASE WHEN maxData % 6 = 1 THEN Drops END) AS Drops1,
		MAX(CASE WHEN maxData % 6 = 2 THEN Drops END) AS Drops2,
		MAX(CASE WHEN maxData % 6 = 3 THEN Drops END) AS Drops3,
		MAX(CASE WHEN maxData % 6 = 4 THEN Drops END) AS Drops4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN Drops END) AS Drops5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN Drops END) AS Drops6,
		
		--|| SpringType Or SpringType ||		
		MAX(CASE WHEN maxData % 6 = 1 THEN SpringType END) AS SpringType1,
		MAX(CASE WHEN maxData % 6 = 2 THEN SpringType END) AS SpringType2,
		MAX(CASE WHEN maxData % 6 = 3 THEN SpringType END) AS SpringType3,
		MAX(CASE WHEN maxData % 6 = 4 THEN SpringType END) AS SpringType4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN SpringType END) AS SpringType5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN SpringType END) AS SpringType6,
		
		--|| Mounting Or Mounting ||		
		MAX(CASE WHEN maxData % 6 = 1 THEN Mounting END) AS Mounting1,
		MAX(CASE WHEN maxData % 6 = 2 THEN Mounting END) AS Mounting2,
		MAX(CASE WHEN maxData % 6 = 3 THEN Mounting END) AS Mounting3,
		MAX(CASE WHEN maxData % 6 = 4 THEN Mounting END) AS Mounting4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN Mounting END) AS Mounting5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN Mounting END) AS Mounting6,
		
		--|| Room Or Room ||		
		MAX(CASE WHEN maxData % 6 = 1 THEN Room END) AS Room1,
		MAX(CASE WHEN maxData % 6 = 2 THEN Room END) AS Room2,
		MAX(CASE WHEN maxData % 6 = 3 THEN Room END) AS Room3,
		MAX(CASE WHEN maxData % 6 = 4 THEN Room END) AS Room4, 
		MAX(CASE WHEN maxData % 6 = 5 THEN Room END) AS Room5, 
		MAX(CASE WHEN maxData % 6 = 0 THEN Room END) AS Room6
		
	FROM
		GroupData 
	GROUP BY
		BlindType,
		amountMax 
	ORDER BY
		BlindType
END;