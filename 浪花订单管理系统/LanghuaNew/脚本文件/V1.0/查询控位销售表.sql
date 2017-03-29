
GO

/****** Object:  StoredProcedure [dbo].[SelectSellControl]    Script Date: 2016/7/29 10:43:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:HELI
-- Create date:2016-07-21
-- Description:	�������ر�id��ȡ��λ���۱�
-- =============================================
CREATE PROCEDURE [dbo].[SelectSellControl]
	@id INT
AS
BEGIN
		
	DECLARE @MonthNum INT
	DECLARE @SellControlNum INT
	DECLARE @StartDate DATETIMEOFFSET
	DECLARE @SupplierID INT    
	DECLARE @FirstServiceItemID INT
	DECLARE @SecondServiceItemID INT
	DECLARE @thisDate DATETIMEOFFSET
    
	SELECT @MonthNum=MonthNum,@SellControlNum=SellControlNum,@StartDate=StartDate,@SupplierID=SupplierID,
		@FirstServiceItemID=FirstServiceItem_ServiceItemID,@SecondServiceItemID=SecondServiceItem_ServiceItemID 
	FROM dbo.SellControls WHERE SellControlID=@id
	--����
	SELECT StartTime,EndTime,RuleUseTypeValue,SelectRuleType,RangeStart,RangeEnd,Week,IsDouble,UseDate
	INTO #RuleTemp
	FROM dbo.ServiceRules a
	JOIN dbo.ServiceRuleServiceItems b ON a.ServiceRuleID = b.ServiceRule_ServiceRuleID
	WHERE (b.ServiceItem_ServiceItemID=@FirstServiceItemID OR b.ServiceItem_ServiceItemID=@SecondServiceItemID)
		AND a.UseState=0  

	CREATE TABLE #Temp(thisdate VARCHAR(10),date VARCHAR(5),TravelNum INT,PreTravelNum INT,ReturnNum INT,PreReturnNum INT,state INT)

	--�������ڼ��������Ž���ʱ��#Temp
	IF(@StartDate>'1900-01-02')
	BEGIN
		set @thisDate=@StartDate
		WHILE(DATEDIFF(d,@thisDate,dateadd(month,@MonthNum,@StartDate)) >0)
		BEGIN
			--PRINT(@thisDate)
			DECLARE @TravelNum INT=0
			DECLARE @PreTravelNum INT=0
			DECLARE @ReturnNum INT=0
			DECLARE @PreReturnNum INT=0
			DECLARE @state INT=0

			--����Ʒ����ȷ��ͳ��
			SELECT @TravelNum+=isnull(SUM(AdultNum)+SUM(ChildNum)+SUM(INFNum),0) FROM (
			SELECT DISTINCT a.OrderID,a.AdultNum,a.ChildNum,a.INFNum 
			FROM dbo.ServiceItemHistories a
			JOIN dbo.Orders b ON a.OrderID = b.OrderID
			WHERE SupplierID=@SupplierID AND ServiceItemID=@FirstServiceItemID 
			AND CONVERT(VARCHAR(10), TravelDate, 120) =CONVERT(VARCHAR(10), @thisDate, 120)
			AND b.state!=11 AND EXISTS(SELECT 1 FROM dbo.OrderHistories WHERE Order_OrderID=a.OrderID AND State=6)  )x
			--����Ʒ����Ԥ��ͳ��		
			SELECT @PreTravelNum+=isnull(SUM(AdultNum)+SUM(ChildNum)+SUM(INFNum),0) FROM (
			SELECT DISTINCT a.OrderID,a.AdultNum,a.ChildNum,a.INFNum 
			FROM dbo.ServiceItemHistories a
			JOIN dbo.Orders b ON a.OrderID = b.OrderID
			WHERE SupplierID=@SupplierID AND ServiceItemID=@FirstServiceItemID 
			AND CONVERT(VARCHAR(10), TravelDate, 120) =CONVERT(VARCHAR(10), @thisDate, 120)
			AND b.state!=11 AND NOT EXISTS(SELECT 1 FROM dbo.OrderHistories WHERE Order_OrderID=a.OrderID AND State=6)  )x

			--�ϲ��Ĳ�Ʒ��Ҫһ��ͳ��
			IF @SecondServiceItemID IS NOT NULL
			BEGIN
					--�β�Ʒ����ȷ��ͳ��
					SELECT @TravelNum+=isnull(SUM(AdultNum)+SUM(ChildNum)+SUM(INFNum),0) FROM (
					SELECT DISTINCT a.OrderID,a.AdultNum,a.ChildNum,a.INFNum 
					FROM dbo.ServiceItemHistories a
					JOIN dbo.Orders b ON a.OrderID = b.OrderID
					WHERE SupplierID=@SupplierID AND ServiceItemID=@SecondServiceItemID 
					AND CONVERT(VARCHAR(10), TravelDate, 120) =CONVERT(VARCHAR(10), @thisDate, 120)
					AND b.state!=11 AND EXISTS(SELECT 1 FROM dbo.OrderHistories WHERE Order_OrderID=a.OrderID AND State=6)  )x
					--�β�Ʒ����Ԥ��ͳ��		
					SELECT @PreTravelNum+=isnull(SUM(AdultNum)+SUM(ChildNum)+SUM(INFNum),0) FROM (
					SELECT DISTINCT a.OrderID,a.AdultNum,a.ChildNum,a.INFNum 
					FROM dbo.ServiceItemHistories a
					JOIN dbo.Orders b ON a.OrderID = b.OrderID
					WHERE SupplierID=@SupplierID AND ServiceItemID=@SecondServiceItemID 
					AND CONVERT(VARCHAR(10), TravelDate, 120) =CONVERT(VARCHAR(10), @thisDate, 120)
					AND b.state!=11 AND NOT EXISTS(SELECT 1 FROM dbo.OrderHistories WHERE Order_OrderID=a.OrderID AND State=6)  )x

					--����Ʒ����ȷ��ͳ��
					SELECT @ReturnNum+=isnull(SUM(AdultNum)+SUM(ChildNum)+SUM(INFNum),0) FROM (
					SELECT DISTINCT a.OrderID,a.AdultNum,a.ChildNum,a.INFNum 
					FROM dbo.ServiceItemHistories a
					JOIN dbo.Orders b ON a.OrderID = b.OrderID
					WHERE SupplierID=@SupplierID AND ServiceItemID=@FirstServiceItemID 
					AND CONVERT(VARCHAR(10), ReturnDate, 120) =CONVERT(VARCHAR(10), @thisDate, 120)
					AND b.state!=11 AND EXISTS(SELECT 1 FROM dbo.OrderHistories WHERE Order_OrderID=a.OrderID AND State=6)  )x
					--����Ʒ����Ԥ��ͳ��		
					SELECT @PreReturnNum+=isnull(SUM(AdultNum)+SUM(ChildNum)+SUM(INFNum),0) FROM (
					SELECT DISTINCT a.OrderID,a.AdultNum,a.ChildNum,a.INFNum 
					FROM dbo.ServiceItemHistories a
					JOIN dbo.Orders b ON a.OrderID = b.OrderID
					WHERE SupplierID=@SupplierID AND ServiceItemID=@FirstServiceItemID 
					AND CONVERT(VARCHAR(10), ReturnDate, 120) =CONVERT(VARCHAR(10), @thisDate, 120)
					AND b.state!=11 AND NOT EXISTS(SELECT 1 FROM dbo.OrderHistories WHERE Order_OrderID=a.OrderID AND State=6)  )x

					--�β�Ʒ����ȷ��ͳ��
					SELECT @ReturnNum+=isnull(SUM(AdultNum)+SUM(ChildNum)+SUM(INFNum),0) FROM (
					SELECT DISTINCT a.OrderID,a.AdultNum,a.ChildNum,a.INFNum 
					FROM dbo.ServiceItemHistories a
					JOIN dbo.Orders b ON a.OrderID = b.OrderID
					WHERE SupplierID=@SupplierID AND ServiceItemID=@SecondServiceItemID 
					AND CONVERT(VARCHAR(10), ReturnDate, 120) =CONVERT(VARCHAR(10), @thisDate, 120)
					AND b.state!=11 AND EXISTS(SELECT 1 FROM dbo.OrderHistories WHERE Order_OrderID=a.OrderID AND State=6)  )x
					--����Ʒ����Ԥ��ͳ��		
					SELECT @PreReturnNum+=isnull(SUM(AdultNum)+SUM(ChildNum)+SUM(INFNum),0) FROM (
					SELECT DISTINCT a.OrderID,a.AdultNum,a.ChildNum,a.INFNum 
					FROM dbo.ServiceItemHistories a
					JOIN dbo.Orders b ON a.OrderID = b.OrderID
					WHERE SupplierID=@SupplierID AND ServiceItemID=@SecondServiceItemID 
					AND CONVERT(VARCHAR(10), ReturnDate, 120) =CONVERT(VARCHAR(10), @thisDate, 120)
					AND b.state!=11 AND NOT EXISTS(SELECT 1 FROM dbo.OrderHistories WHERE Order_OrderID=a.OrderID AND State=6)  )x
			END
			ELSE
			BEGIN
				SET @ReturnNum=-1
				SET @PreReturnNum=-1
			END	


			--�������������ɲ�ͬ��ɫ 0��ɫ 1��ɫ 2��ɫ 3��ɫ 4��ɫ
			IF @TravelNum>@SellControlNum
				SET @state=3
			ELSE IF @TravelNum+@PreTravelNum>@SellControlNum
				SET @state=2
			ELSE IF @TravelNum>0
				SET @state=1

			IF @ReturnNum>@SellControlNum
				SET @state=3
			ELSE IF @ReturnNum+@PreReturnNum>@SellControlNum
				SET @state=CASE WHEN @state>2 THEN @state ELSE 2 END
			ELSE IF @ReturnNum>0
				SET @state=CASE WHEN @state>1 THEN @state ELSE 1 END
			 --���ݹ�������������thisdate�Ƿ��ֹ
			DECLARE rule_Cur CURSOR
			FOR
			SELECT StartTime,EndTime,RuleUseTypeValue,SelectRuleType,RangeStart,RangeEnd,Week,IsDouble,UseDate
			FROM #RuleTemp              
					    
			OPEN rule_Cur

			DECLARE @StartTime DATETIMEOFFSET
			DECLARE @EndTime DATETIMEOFFSET
			DECLARE @RuleUseTypeValue INT
			DECLARE @SelectRuleType INT
			DECLARE @RangeStart DATETIMEOFFSET
			DECLARE @RangeEnd DATETIMEOFFSET
			DECLARE @Week VARCHAR(50)
			DECLARE @IsDouble BIT
			DECLARE @UseDate VARCHAR(50)

			FETCH rule_Cur INTO @StartTime,@EndTime,@RuleUseTypeValue,@SelectRuleType,@RangeStart,@RangeEnd,@Week,@IsDouble,@UseDate

			WHILE(@@fetch_status=0)
			BEGIN
				IF @thisDate>=@StartTime AND @thisDate<=@EndTime
				BEGIN
					IF @SelectRuleType=0 AND ((@thisDate>=@RangeStart AND @thisDate<=@RangeEnd AND @RuleUseTypeValue=1)--��ֹ�ڷ�Χ��
								OR ((@thisDate<@RangeStart OR @thisDate>@RangeEnd) AND @RuleUseTypeValue=0))--ֻ�����ڷ�Χ��
						SET @state=4
					IF @SelectRuleType=1 AND ((EXISTS(SELECT 1 FROM ufn_SplitStringToTable(@Week,'|') WHERE id=datepart(weekday,@thisDate)-1) AND @RuleUseTypeValue=1)--��ֹ�����ڼ�
								OR (NOT EXISTS(SELECT 1 FROM ufn_SplitStringToTable(@Week,'|') WHERE id=datepart(weekday,@thisDate)-1) AND @RuleUseTypeValue=0))--ֻ���������ڼ�
						SET @state=4
					IF @SelectRuleType=2 AND (( ((Datename(dd,@thisDate)%2=0 AND @IsDouble='true')or(Datename(dd,@thisDate)%2=1 AND @IsDouble='false')) AND @RuleUseTypeValue=1)--��ֹ����˫
								OR (((Datename(dd,@thisDate)%2=0 AND @IsDouble='false')or(Datename(dd,@thisDate)%2=1 AND @IsDouble='true')) AND @RuleUseTypeValue=0))--ֻ������˫
						SET @state=4
					IF @SelectRuleType=3 AND ((EXISTS(SELECT 1 FROM ufn_SplitStringToTable(@UseDate,'|') WHERE id=Datename( dd,@thisDate)) AND @RuleUseTypeValue=1)--��ֹ����
								OR (NOT EXISTS(SELECT 1 FROM ufn_SplitStringToTable(@UseDate,'|') WHERE id=Datename( dd,@thisDate)) AND @RuleUseTypeValue=0))--ֻ������
						SET @state=4

					IF @state=4 BREAK
				END

				fetch next FROM rule_Cur INTO @StartTime,@EndTime,@RuleUseTypeValue,@SelectRuleType,@RangeStart,@RangeEnd,@Week,@IsDouble,@UseDate
			END	
			close rule_Cur
			deallocate rule_Cur


			INSERT INTO #Temp
					(	thisdate ,
						date ,
						TravelNum ,
						PreTravelNum ,
						ReturnNum ,
						PreReturnNum ,
						state
					)
			VALUES  (	CONVERT(VARCHAR(10),@thisDate, 120)  , -- thisdate - varchar(10)
						Datename(month,@thisDate)+'-'+ CASE WHEN  Datename( dd,@thisDate)<10 THEN '0'+Datename( dd,@thisDate) ELSE Datename( dd,@thisDate) END , -- date - varchar(5)
						@TravelNum , -- TravelNum - int
						@PreTravelNum , -- PreTravelNum - int
						@ReturnNum , -- ReturnNum - int
						@PreReturnNum , -- PreReturnNum - int
						@state  -- state - int
					)
			SET @thisDate=dateadd(dd,1,@thisDate)
		END
		
	END
	SELECT * FROM #Temp

END


GO


