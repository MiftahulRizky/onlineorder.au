Imports System.Data
Imports System.Data.SqlClient

Partial Public Class SiteMaster
    Inherits MasterPage

    Dim publicCfg As New PublicConfig
    Dim orderCfg As New OrderConfig
    Dim settingCfg As New SettingConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString


    Protected Sub Page_Init(sender As Object, e As EventArgs)
        AddHandler Page.PreLoad, AddressOf master_Page_PreLoad
    End Sub

    Protected Sub master_Page_PreLoad(sender As Object, e As EventArgs)
        CheckSessions(Session("IsLoggedIn"))

        MyLoad()
        BindListNavigation()
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs)
        CheckSessions(Session("IsLoggedIn"))
        BindActiveNavigasi()
    End Sub

    Protected Sub btnSearchAll_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/account/login", False)
    End Sub

    Protected Sub linkOrder_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/order", False)
    End Sub

    Private Sub MyLoad()
        Try
            If Session("isLoggedIn") = True Then
                Dim loginId As String = Session("LoginId")
                Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM view_auth WHERE Id = '" + loginId + "'")

                Session("FullName") = myData.Tables(0).Rows(0).Item("FullName").ToString()
                Session("AppName") = myData.Tables(0).Rows(0).Item("AppName").ToString()
                Session("RoleId") = myData.Tables(0).Rows(0).Item("RoleId").ToString()
                Session("RoleName") = myData.Tables(0).Rows(0).Item("RoleName").ToString()
                Session("LevelId") = myData.Tables(0).Rows(0).Item("LevelId").ToString()
                Session("LevelName") = myData.Tables(0).Rows(0).Item("LevelName").ToString()
                Session("CustomerId") = myData.Tables(0).Rows(0).Item("CustomerId").ToString()
                Session("CustomerCompany") = myData.Tables(0).Rows(0).Item("CustomerCompany").ToString()
                Session("resetLogin") = myData.Tables(0).Rows(0).Item("Reset")
                Session("CustomerAccount") = myData.Tables(0).Rows(0).Item("CustomerAccount").ToString()

                Dim myData2 As DataSet = publicCfg.GetListData("SELECT * FROM CustomerContacts WHERE CustomerId = '" + Session("CustomerId") + "' AND Name = '" + Session("FullName") + "'")
                IF myData2.Tables(0).Rows.Count > 0 Then
                    Session("CustomerContactId") = myData2.Tables(0).Rows(0).Item("Id").ToString()
                    Session("PriceAccess") = myData2.Tables(0).Rows(0).Item("Price").ToString()
                    Session("MarkUpAccess") = myData2.Tables(0).Rows(0).Item("MarkUp").ToString()
                End If

                Dim myData3 As Boolean = orderCfg.GetItemData_Boolean("SELECT OnStop FROM Customers WHERE Id='" + Session("CustomerId") + "'")
                Session("OnStop") = myData3

                Dim appActive As Boolean = myData.Tables(0).Rows(0).Item("AppActive")
                Dim customerActive As Boolean = myData.Tables(0).Rows(0).Item("Active")
                Dim roleActive As Boolean = myData.Tables(0).Rows(0).Item("RoleActive")
                Dim levelActive As Boolean = myData.Tables(0).Rows(0).Item("LevelActive")
                Dim resetLogin As Boolean = myData.Tables(0).Rows(0).Item("Reset")

                ' If resetLogin = True AndAlso Not Request.Url.AbsolutePath.ToLower().EndsWith("/account/password") Then
                '     Response.Redirect("~/account/password", False)
                '     Exit Sub
                ' End If

                If appActive = False Then
                    Response.Redirect("~/system/maintenance", False)
                    Exit Sub
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerLogins SET LastLogin=GETDATE() WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", loginId)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

            End If

        Catch ex As Exception
            ' publicCfg.MailError(Session("LoginId"), Page.Title, "MyLoad", ex.ToString())
            ' Session.Clear()
            ' Response.Redirect("~/account/login", False)
            Call MessageError(True, String.Empty, ex.ToString())
            Exit Sub
        End Try
    End Sub

    Private Sub BindListNavigation()
        Try
            liOrder.Visible = False
            liShipment.Visible = False '#shutter
            liExport.Visible = False

            liExportShutter.Visible = False '#shutter
            aExportBoe.Visible = True
            aExportLS.Visible = True
            aExportSP.Visible = True

            liImport.Visible = False

            liSales.Visible = False '#shutter
            liReport.Visible = False '#shutter

            liStatistic.Visible = False
            liTutorial.Visible = False

            liSettingCustomer.Visible = False
            liSetting.Visible = False

            divSystem.Visible = False

            divAccess.Visible = False
            dividerUsers.Visible = False
            aRegion.Visible = False
            aCompany.Visible = False

            aCustomer.Visible = False '#shutter
            aCustomerGroup.Visible = False '#shutter
            aCustomerLogin.Visible = False '#shutter
            aCustomerPriceGroup.Visible = False '#shutter
            divDividerCustomer.Visible = False '#shutter
            divDividerCustomerDisc.Visible = False '#shutter
            aCustomerDiscount.Visible = False '#shutter
            divCustomerAdmin.Visible = False '#shutter

            divProduct.Visible = False
            aDesign.Visible = False
            aBlind.Visible = False
            aKit.Visible = False
            aFabric.Visible = False
            aChain.Visible = False
            aBottom.Visible = False

            aProduct.Visible = False
            aMounting.Visible = False

            divPrice.Visible = False
            aPriceGroup.Visible = False
            aPriceMatrix.Visible = False
            aCassetteExtra.Visible = False

            aPriceGroup2.Visible = False
            aPriceMatrix2.Visible = False

            divLog.Visible = False
            divOther.Visible = False

            aFeedback.Visible = False
            aDeleteFile.Visible = False
            aDeleteOrder.Visible = False
            aQuery.Visible = False

            spanOrder.InnerText = "Create & View Order"

            If Session("RoleName") = "Administrator" Then
                liOrder.Visible = True

                If Session("CustomerCompany") = "SP" Then
                    liExport.Visible = True
                End If

                liShipment.Visible = True '#shutter
                liExportShutter.Visible = True '#shutter
                aExportBoe.Visible = True '#shutter
                aExportLS.Visible = True '#shutter
                aExportSP.Visible = True '#shutter
                liSales.Visible = True '#shutter
                liReport.Visible = True '#shutter
                
                liSetting.Visible = True

                divSystem.Visible = True

                divAccess.Visible = True
                dividerUsers.Visible = True
                aRegion.Visible = True
                aCompany.Visible = True

                divCustomerAdmin.Visible = True '#shutter

                divProduct.Visible = True
                aDesign.Visible = True
                aBlind.Visible = True
                aKit.Visible = True
                aFabric.Visible = True
                aChain.Visible = True
                aBottom.Visible = True

                aProduct.Visible = True
                aMounting.Visible = True

                divPrice.Visible = True
                aPriceGroup.Visible = True
                aPriceMatrix.Visible = True
                aCassetteExtra.Visible = True

                aPriceGroup2.Visible = True
                aPriceMatrix2.Visible = True

                divLog.Visible = True
                divOther.Visible = True

                aFeedback.Visible = True
                aDeleteFile.Visible = True
                aDeleteOrder.Visible = True
                aQuery.Visible = True
            End If

            If Session("RoleName") = "Customer Service" Then
                liOrder.Visible = True
                liShipment.Visible = True
                liReport.Visible = True
                liSales.Visible = True

                liSetting.Visible = True

                aCustomer.Visible = True
                aCustomerGroup.Visible = True
                aCustomerPriceGroup.Visible = True
                divDividerCustomer.Visible = True
                divDividerCustomerDisc.Visible = True
                aCustomerDiscount.Visible = True
            End If

            If Session("RoleName") = "Data Entry" Then
                liOrder.Visible = True
                liReport.Visible = True
            End If

            If Session("RoleName") = "Representative" Then
                liOrder.Visible = True

                liSetting.Visible = True

                aCustomer.Visible = True
                divDividerCustomerDisc.Visible = True
                aCustomerDiscount.Visible = True
            End If

            If Session("RoleName") = "PPIC & DE" Or Session("RoleName") = "Manager" Then
                liOrder.Visible = True
                liReport.Visible = True
            End If

            If Session("RoleName") = "Customer" Then
                spanOrder.InnerText = "My Order"

                liOrder.Visible = True
                liSettingCustomer.Visible = True
            End If

            If Session("RoleName") = "Account" Then
                liOrder.Visible = True
                liSales.Visible = True
                liReport.Visible = True
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
       Dim sessionId As String = String.Empty

        If Request.Cookies("deviceId") IsNot Nothing Then
            sessionId = Request.Cookies("deviceId").Value
            settingCfg.DeleteSession(sessionId)
        End If

        Session.Clear()
        Response.Redirect("~/", False)
    End Sub

    Private Sub CheckSessions(IsLogged As Boolean)
        Try
            Dim sessionId As String = String.Empty
            If IsLogged = True Then
                If Request.Cookies("deviceId") IsNot Nothing Then
                    sessionId = Request.Cookies("deviceId").Value
                    Dim checkData As DataSet = publicCfg.GetListData("SELECT * FROM Sessions WHERE Id = '" + UCase(sessionId) + "' AND LoginId = '" + UCase(Session("LoginId")) + "'")
                    If checkData.Tables(0).Rows.Count = 0 Then
                        Response.Redirect("~/account/login", False)
                        Exit Sub
                    End If
                Else
                    Response.Redirect("~/account/login", False)
                    Exit Sub
                End If
            Else
                If Request.Cookies("deviceId") IsNot Nothing Then
                    sessionId = Request.Cookies("deviceId").Value
                    Dim loginId As String = publicCfg.GetItemData("SELECT LoginId FROM Sessions WHERE Id = '" + UCase(sessionId).ToString() + "'")
                    If Not loginId = "" Then
                        Dim appId As String = publicCfg.GetItemData("SELECT ApplicationId FROM CustomerLogins WHERE Id = '" + UCase(loginId).ToString() + "'")
                        Dim userName As String = publicCfg.GetItemData("SELECT UserName FROM CustomerLogins WHERE Id = '" + UCase(loginId).ToString() + "'")

                        Session.Add("IsLoggedIn", True)
                        Session.Add("LoginId", UCase(loginId).ToString())
                        Session.Add("ApplicationId", UCase(appId).ToString())
                        Session.Add("UserName", userName)

                        Response.Redirect("~/", False)
                        Exit Sub
                    Else
                        Response.Redirect("~/account/login", False)
                        Exit Sub
                    End If
                Else
                    Response.Redirect("~/account/login", False)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            Exit Sub
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, input As String, Msg As String)
        ' divError.Visible = False 
        ' msgError.InnerText = Msg
        If Show = True Then 
            ' divError.Visible = True 
            Dim escapedMsg As String = HttpUtility.JavaScriptStringEncode(Msg)
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Script", "showMessageError('"& escapedMsg &"', '"& input &"')", True)
        End If
    End Sub
End Class