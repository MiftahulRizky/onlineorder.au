Imports System.Data

Partial Public Class SiteMaster
    Inherits MasterPage

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Init(sender As Object, e As EventArgs)
        AddHandler Page.PreLoad, AddressOf master_Page_PreLoad
    End Sub

    Protected Sub master_Page_PreLoad(sender As Object, e As EventArgs)
        If Not Session("isLoggedIn") = True Then
            Response.Redirect("~/account/login", False)
            Exit Sub
        End If

        Call MyLoad()
        Call BindListNavigation()
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs)
        Call BindActiveNavigasi()
    End Sub

    Protected Sub btnSearchAll_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/account/login", False)
    End Sub

    Private Sub MyLoad()
        Try
            Dim checkApplication As Boolean = publicCfg.GetConfig(Session("ApplicationId").ToString())
            If checkApplication = False Then
                Response.Redirect("~/error/maintenance", False)
                Exit Sub
            End If

            lblUserId.Text = Session("UserId")

            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM view_memberships WHERE UserId = '" + lblUserId.Text + "'")

            Session("AppName") = myData.Tables(0).Rows(0).Item("AppName").ToString()

            Session("LevelName") = myData.Tables(0).Rows(0).Item("LevelName").ToString()

            Session("RoleId") = myData.Tables(0).Rows(0).Item("RoleId").ToString()
            Session("RoleName") = myData.Tables(0).Rows(0).Item("RoleName").ToString()

            Session("StoreId") = myData.Tables(0).Rows(0).Item("StoreId").ToString()
            Session("UserEmail") = myData.Tables(0).Rows(0).Item("UserEmail").ToString()
            Session("MarkUpAccess") = myData.Tables(0).Rows(0).Item("MarkUpAccess")
            Session("PriceAccess") = myData.Tables(0).Rows(0).Item("PriceAccess")

            Session("StoreName") = myData.Tables(0).Rows(0).Item("StoreName").ToString()
            Session("StoreCompany") = myData.Tables(0).Rows(0).Item("StoreCompany").ToString()
            Session("StoreType") = myData.Tables(0).Rows(0).Item("StoreType").ToString()
            Session("StoreEmail") = myData.Tables(0).Rows(0).Item("StoreEmail").ToString()
            Session("StoreImage") = myData.Tables(0).Rows(0).Item("StoreImage").ToString()

            If Session("StoreCompany") = "SG" Then
                imgLogo.ImageUrl = "~/Content/static/sg_logo.jpg"
            End If

            sdsLastLogin.Update()
        Catch ex As Exception
            publicCfg.MailError(Session("UserId"), Page.Title, "MyLoad", ex.ToString())
        End Try
    End Sub

    Private Sub BindListNavigation()
        Try
            liOrder.Visible = True
            liExport.Visible = True
            liImport.Visible = True
            liStatistic.Visible = True
            liTutorial.Visible = True

            liSetting.Visible = True

            aApplication.Visible = True
            aEmail.Visible = True

            divUsers.Visible = True
            aStore.Visible = True
            aMembership.Visible = True
            dividerUsers.Visible = True
            aRole.Visible = True
            aMemberLevel.Visible = True
            aRegion.Visible = True
            aCompany.Visible = True

            divProduct.Visible = True
            aDesign.Visible = True
            aBlind.Visible = True
            aKit.Visible = True
            aFabric.Visible = True
            aChain.Visible = True
            aBottom.Visible = True

            divPrice.Visible = True
            aPriceGroup.Visible = True
            aPriceMatrix.Visible = True
            aCassetteExtra.Visible = True

            aFeedback.Visible = True
            aDeleteFile.Visible = True
            aDeleteOrder.Visible = True
            aQuery.Visible = True

            If Session("RoleName") = "PPIC & DE" Or Session("RoleName") = "Manager" Then
                liExport.Visible = False
                liSetting.Visible = False
            End If

            If Session("RoleName") = "Customer" Then
                liExport.Visible = False
                liImport.Visible = False
                liStatistic.Visible = False
                liSetting.Visible = False
            End If

            If Session("RoleName") = "Account" Then
                liExport.Visible = False
                liImport.Visible = False
                liStatistic.Visible = False
                aApplication.Visible = False
                aEmail.Visible = False
                divProduct.Visible = False
                aFeedback.Visible = False
                aDeleteFile.Visible = False
                aDeleteOrder.Visible = False
                aQuery.Visible = False
                '#masteruser/
                aStore.Visible = False
                dividerUsers.Visible = False
                aRole.Visible = False
                aMemberLevel.Visible = False
                aRegion.Visible = False
                aCompany.Visible = False
                '#masterprice/ 
                aPriceGroup.Visible = False
                aPriceMatrix.Visible = False
                aCassetteExtra.Visible = False
                aSurcharge.Visible = False

            End If
        Catch ex As Exception
            publicCfg.MailError(Session("UserId"), Page.Title, "BindListNavigation", ex.ToString())
        End Try
    End Sub

    Private Sub BindActiveNavigasi()
        Try
            If Page.Title = "Home Page" Or Page.Title = "About Us" Or Page.Title = "Contact Us" Or Page.Title = "My Activity" Or Page.Title = "Faq" Then
                liHome.Attributes.Add("class", "nav-item active")
            End If

            If Page.Title = "List Order" Or Page.Title = "Add Item Order" Or Page.Title = "Create Header" Or Page.Title = "Detail Order" Or Page.Title = "Change Status" Or Page.Title = "Aluminium Order" Or Page.Title = "Cellular Shades Order" Or Page.Title = "Pelmet Order" Or Page.Title = "Roller Order" Or Page.Title = "Venetian Order" Or Page.Title = "Veri Shades Order" Or Page.Title = "Vertical Order" Or Page.Title = "Maintenance Order" Or Page.Title = "Edit Header" Or Page.Title = "Quote Detail" Then
                liOrder.Attributes.Add("class", "nav-item active")
            End If

            If Page.Title = "Export Order" Then
                liExport.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Import Order" Then
                liImport.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Statistics" Then
                liStatistic.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Tutorial" Or Page.Title = "Add Tutorial" Or Page.Title = "Edit Tutorial" Or Page.Title = "View Tutorial" Then
                liTutorial.Attributes.Add("class", "nav-item dropdown active")
            End If

            ' SETTING
            If Page.Title = "Application" Or Page.Title = "Add Application" Or Page.Title = "Detail Application" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Mail Configuration" Or Page.Title = "Add Mail Configuration" Or Page.Title = "Detail Mail Configuration" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Store" Or Page.Title = "Add Store" Or Page.Title = "Detail Store" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Membership" Or Page.Title = "Add Membership" Or Page.Title = "Detail Membership" Or Page.Title = "Online Membership" Or Page.Title = "Activity Membership" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Role" Or Page.Title = "Add Role" Or Page.Title = "Detail Role" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Member Level" Or Page.Title = "Add Member Level" Or Page.Title = "Detail Member Level" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Region" Or Page.Title = "Add Region" Or Page.Title = "Detail Region" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Company" Or Page.Title = "Add Company" Or Page.Title = "Detail Company" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            ' SETTING PRODUCT
            If Page.Title = "Setting" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Design Type" Or Page.Title = "Add Design Type" Or Page.Title = "Detail Design Type" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Blind Type" Or Page.Title = "Add Blind Type" Or Page.Title = "Detail Blind Type" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Hardware Kit" Or Page.Title = "Add Hardware Kit" Or Page.Title = "Detail Hardware Kit" Or Page.Title = "Hardware Kit SG" Or Page.Title = "Add Hardware Kit SG" Or Page.Title = "Detail Hardware Kit SG" Or Page.Title = "Bracket Type" Or Page.Title = "Tube Type" Or Page.Title = "Control Type" Or Page.Title = "Colour Type" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Fabric" Or Page.Title = "Add Fabric" Or Page.Title = "Detail Fabric" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Chain" Or Page.Title = "Add Chain" Or Page.Title = "Detail Chain" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Bottom Rail" Or Page.Title = "Add Bottom Rail" Or Page.Title = "Detail Bottom Rail" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Notification" Or Page.Title = "Add Notification" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "FeedBack" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            ' SETTING PRICING
            If Page.Title = "Price Group" Or Page.Title = "Add Price Group" Or Page.Title = "Import Price Group" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Price Matrix" Or Page.Title = "Add Price Matrix" Or Page.Title = "Import Price Matrix" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Surcharge" Or Page.Title = "Add Surcharge" Or Page.Title = "Detail Surcharge" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Discount" Or Page.Title = "Add Discount" Or Page.Title = "Detail Discount" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If

            If Page.Title = "Query" Then
                liSetting.Attributes.Add("class", "nav-item dropdown active")
            End If


            '#Setting/Application



        Catch ex As Exception
            'publicCfg.MailError(Session("UserId"), Page.Title, "BindActiveNavigasi", ex.ToString())
        End Try
    End Sub

    Protected Sub linkLogout_Click(sender As Object, e As EventArgs)
        Session.Clear()
        Response.Redirect("~/", False)
    End Sub
End Class