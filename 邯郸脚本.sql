USE [dbLabelInfo]
GO
/****** Object:  Table [dbo].[NAP_ParentPoints]    Script Date: 2022/9/25 16:47:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NAP_ParentPoints](
	[ID] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[TypeId] [int] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[UpdateTime] [datetime] NULL,
 CONSTRAINT [PK_NAP_ParentPoints] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NAP_People]    Script Date: 2022/9/25 16:47:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NAP_People](
	[ID] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Sex] [nvarchar](10) NULL,
	[Nation] [nvarchar](10) NULL,
	[Birthday] [date] NULL,
	[Address] [nvarchar](50) NULL,
	[IDPhoto] [nvarchar](2000) NULL,
	[ContactInfo] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[UpdateTime] [datetime] NULL,
 CONSTRAINT [PK_NAP_People] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NAP_PickPoints]    Script Date: 2022/9/25 16:47:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NAP_PickPoints](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[UpdateTime] [datetime] NULL,
 CONSTRAINT [PK_NAP_PickPoints] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NAP_PickRecords]    Script Date: 2022/9/25 16:47:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NAP_PickRecords](
	[TubeId] [int] NOT NULL,
	[PersonId] [nvarchar](50) NOT NULL,
	[PickDate] [date] NOT NULL,
 CONSTRAINT [PK_NAP_PickRecords] PRIMARY KEY CLUSTERED 
(
	[TubeId] ASC,
	[PersonId] ASC,
	[PickDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NAP_PointTypes]    Script Date: 2022/9/25 16:47:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NAP_PointTypes](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[UpdateTime] [datetime] NULL,
 CONSTRAINT [PK_NAP_PointTypes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NAP_Tubes]    Script Date: 2022/9/25 16:47:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NAP_Tubes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[BarCode] [nvarchar](50) NOT NULL,
	[LabelStatus] [int] NOT NULL,
	[LabelTime] [datetime] NULL,
	[ResponseStatus] [int] NOT NULL,
	[ResponseTime] [datetime] NULL,
	[UserId] [nvarchar](20) NOT NULL,
	[PPId] [int] NOT NULL,
	[LS01] [nvarchar](200) NULL,
	[LS02] [nvarchar](200) NULL,
	[LS03] [nvarchar](200) NULL,
	[LS04] [nvarchar](200) NULL,
	[LS05] [nvarchar](200) NULL,
	[LS06] [nvarchar](200) NULL,
	[LS07] [nvarchar](200) NULL,
	[LS08] [nvarchar](200) NULL,
	[LS09] [nvarchar](200) NULL,
	[LS10] [nvarchar](200) NULL,
	[CreateTime] [datetime] NOT NULL,
	[UpdateTime] [datetime] NULL,
 CONSTRAINT [PK_NAP_Tubes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NAP_UserPickPoints]    Script Date: 2022/9/25 16:47:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NAP_UserPickPoints](
	[UserId] [nvarchar](20) NOT NULL,
	[PPId] [int] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[UpdateTime] [datetime] NULL,
 CONSTRAINT [PK_NAP_UserPickPoints] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[PPId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[NAP_ParentPoints] ADD  CONSTRAINT [DF_NAP_ParentPoints_CreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].[NAP_People] ADD  CONSTRAINT [DF_NAP_People_CreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].[NAP_PickPoints] ADD  CONSTRAINT [DF_NAP_PickPoints_CreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].[NAP_PointTypes] ADD  CONSTRAINT [DF_NAP_PointTypes_CreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].[NAP_Tubes] ADD  CONSTRAINT [DF_NAP_Tubes_LabelStatus]  DEFAULT ((0)) FOR [LabelStatus]
GO
ALTER TABLE [dbo].[NAP_Tubes] ADD  CONSTRAINT [DF_NAP_Tubes_ResponseStatus]  DEFAULT ((0)) FOR [ResponseStatus]
GO
ALTER TABLE [dbo].[NAP_Tubes] ADD  CONSTRAINT [DF_NAP_Tubes_CreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].[NAP_UserPickPoints] ADD  CONSTRAINT [DF_NAP_UserPickPoints_CreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'身份证号码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_People', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_People', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'性别' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_People', @level2type=N'COLUMN',@level2name=N'Sex'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'民族' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_People', @level2type=N'COLUMN',@level2name=N'Nation'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'生日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_People', @level2type=N'COLUMN',@level2name=N'Birthday'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_People', @level2type=N'COLUMN',@level2name=N'Address'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'照片' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_People', @level2type=N'COLUMN',@level2name=N'IDPhoto'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系方式' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_People', @level2type=N'COLUMN',@level2name=N'ContactInfo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_PickPoints', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_PickPoints', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'条码前缀' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_PickPoints', @level2type=N'COLUMN',@level2name=N'Code'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'试管id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_PickRecords', @level2type=N'COLUMN',@level2name=N'TubeId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'人id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_PickRecords', @level2type=N'COLUMN',@level2name=N'PersonId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'日期 到日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_PickRecords', @level2type=N'COLUMN',@level2name=N'PickDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'条码号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_Tubes', @level2type=N'COLUMN',@level2name=N'BarCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'打印状态[0-未打印 1-打印完成]' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_Tubes', @level2type=N'COLUMN',@level2name=N'LabelStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'打印完成时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_Tubes', @level2type=N'COLUMN',@level2name=N'LabelTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'签收状态[0-未签收 1-签收完成]，打印完成才签收' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_Tubes', @level2type=N'COLUMN',@level2name=N'ResponseStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'签收时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_Tubes', @level2type=N'COLUMN',@level2name=N'ResponseTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'操作人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_Tubes', @level2type=N'COLUMN',@level2name=N'UserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'采样点Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_Tubes', @level2type=N'COLUMN',@level2name=N'PPId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标签信息1-10，第一个固定为条码号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_Tubes', @level2type=N'COLUMN',@level2name=N'LS01'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'操作人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_UserPickPoints', @level2type=N'COLUMN',@level2name=N'UserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'采样点Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NAP_UserPickPoints', @level2type=N'COLUMN',@level2name=N'PPId'
GO
