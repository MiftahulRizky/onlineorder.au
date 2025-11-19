
Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Product_Fabric_Detail
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/product", False)
            Exit Sub
        End If

        If Session("fabricDetail") = "" Then
            Response.Redirect("~/setting/product/fabric", False)
            Exit Sub
        End If

        lblId.Text = Session("fabricDetail")
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            MessageError_Edit(False, String.Empty)
            BindDataFabric(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmitEdit_Click(sender As Object, e As EventArgs)
        MessageError_Edit(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showEdit(); };"
        Try
            Dim design As String = String.Empty
            For Each item As ListItem In lbDesign.Items
                If item.Selected Then
                    design += UCase(item.Value).ToString() & ","
                End If
            Next
            If design = "" Then
                MessageError_Edit(True, "DESIGN TYPE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showAdd()", thisScript, True)
                lbDesign.BackColor = Drawing.Color.Red
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim thisId As String = settingCfg.CreateFabricId()
                Dim designType As String = design.Remove(design.Length - 1).ToString()

                Dim tubeType As String = String.Empty
                Dim tube As String = String.Empty
                If Not lbTube.SelectedValue = "" Then
                    For Each item As ListItem In lbTube.Items
                        If item.Selected Then
                            tube += item.Value & ","
                        End If
                    Next
                    tubeType = tube.Remove(tube.Length - 1).ToString()
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE Fabrics SET DesignId=@DesignId, TubeType=@TubeType, Factory=@Factory, Name=@Name, Type=@Type, [Group]=@Group, Composition=@Composition, FlameReterdant=@FlameReterdant, PvcLead=@PvcLead, GreenguardGold=@GreenguardGold, Weight=@Weight, Thickness=@Thickness, NoRailRoad=@NoRailRoad, Active=@Active WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        myCmd.Parameters.AddWithValue("@DesignId", designType)
                        myCmd.Parameters.AddWithValue("@TubeType", If(String.IsNullOrEmpty(tubeType), DBNull.Value, tubeType))
                        myCmd.Parameters.AddWithValue("@Factory", ddlFactory.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Group", txtGroup.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Composition", If(String.IsNullOrEmpty(txtComposition.Text), DBNull.Value, txtComposition.Text))
                        myCmd.Parameters.AddWithValue("@FlameReterdant", If(String.IsNullOrEmpty(ddlFlameReterdant.SelectedValue), 0, ddlFlameReterdant.SelectedValue))
                        myCmd.Parameters.AddWithValue("@PvcLead", If(String.IsNullOrEmpty(ddlPvcLead.SelectedValue), DBNull.Value, ddlPvcLead.SelectedValue))
                        myCmd.Parameters.AddWithValue("@GreenguardGold", If(String.IsNullOrEmpty(ddlGreenguardGold.SelectedValue), 0, ddlGreenguardGold.SelectedValue))
                        myCmd.Parameters.AddWithValue("@Weight", If(String.IsNullOrEmpty(txtWeight.Text), 0, txtWeight.Text))
                        myCmd.Parameters.AddWithValue("@Thickness", If(String.IsNullOrEmpty(txtThickness.Text), 0, txtThickness.Text))
                        myCmd.Parameters.AddWithValue("@NoRailRoad", ddlNoRailRoad.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                Dim dataLog As Object() = {"Fabrics", thisId, Session("LoginId").ToString(), "Fabric Updated"}
                settingCfg.Log_Product(dataLog)

                Response.Redirect("~/setting/product/fabric/detail", False)
            End If
        Catch ex As Exception
            MessageError_Edit(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showEdit", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSubmitDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Fabrics WHERE Id=@Id DELETE FROM FabricColours WHERE FabricId=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Response.Redirect("~/setting/product/fabric", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnLog_Click(sender As Object, e As EventArgs)
        MessageError_Log(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showLog(); };"
        Try
            gvListLogs.DataSource = settingCfg.GetListData("SELECT * FROM Log_Products WHERE DataId = '" + lblId.Text + "' AND Type='Fabrics' ORDER BY ActionDate DESC")
            gvListLogs.DataBind()

            ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
        Catch ex As Exception
            MessageError_Log(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
        End Try
    End Sub

    Protected Sub btnAddColour_Click(sender As Object, e As EventArgs)
        MessageError_AddColour(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showAddColour(); };"
        Try
            lblActionColour.Text = "Add"
            titleColour.InnerText = "Add Fabric Colour"
            ClientScript.RegisterStartupScript(Me.GetType(), "showAddColour", thisScript, True)
        Catch ex As Exception
            MessageError_AddColour(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showAddColour", thisScript, True)
        End Try
    End Sub

    Protected Sub gvListColour_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvListColour.PageIndex = e.NewPageIndex
            BindDataFabric(lblId.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub gvListColour_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_AddColour(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showAddColour(); };"
                Try
                    lblIdColour.Text = dataId
                    lblActionColour.Text = "Edit"
                    titleColour.InnerText = "Edit Fabric Colour"

                    Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM FabricColours WHERE Id = '" + lblIdColour.Text + "'")

                    txtBoeId.Text = myData.Tables(0).Rows(0).Item("BoeId").ToString()
                    txtFabricColourName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
                    txtNameColour.Text = myData.Tables(0).Rows(0).Item("Colour").ToString()
                    txtColourWidth.Text = myData.Tables(0).Rows(0).Item("Width").ToString()
                    ddlColourActive.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Active"))

                    ClientScript.RegisterStartupScript(Me.GetType(), "showAddColour()", thisScript, True)
                Catch ex As Exception
                    MessageError_AddColour(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showAddColour()", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingCfg.GetListData("SELECT * FROM Log_Products WHERE DataId = '" + dataId + "' AND Type='FabricColours' ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnSubmitAddColour_Click(sender As Object, e As EventArgs)
        MessageError_AddColour(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showAddColour(); };"
        Try
            If msgErrorAddColour.InnerText = "" Then
                If lblActionColour.Text = "Add" Then
                    Dim thisId As String = settingCfg.CreateFabricColourId()

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO FabricColours VALUES (@Id, @FabricId, @BoeId, @Name, @Colour, @Width, @Active)")
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@FabricId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@BoeId", If(String.IsNullOrEmpty(txtBoeId.Text), DBNull.Value, txtBoeId.Text))
                            myCmd.Parameters.AddWithValue("@Name", txtFabricColourName.Text)
                            myCmd.Parameters.AddWithValue("@Colour", txtNameColour.Text)
                            myCmd.Parameters.AddWithValue("@Width", txtColourWidth.Text)
                            myCmd.Parameters.AddWithValue("@Active", ddlColourActive.SelectedValue)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"FabricColours", thisId, Session("LoginId").ToString(), "Fabric Colour Created"}
                    settingCfg.Log_Product(dataLog)

                    Response.Redirect("~/setting/product/fabric/detail", False)
                    Exit Sub
                End If

                If lblActionColour.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE FabricColours SET BoeId=@BoeId, Name=@Name, Colour=@Colour, Width=@Width, Active=@Active WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", lblIdColour.Text)
                            myCmd.Parameters.AddWithValue("@FabricId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@BoeId", If(String.IsNullOrEmpty(txtBoeId.Text), DBNull.Value, txtBoeId.Text))
                            myCmd.Parameters.AddWithValue("@Name", txtFabricColourName.Text)
                            myCmd.Parameters.AddWithValue("@Colour", txtNameColour.Text)
                            myCmd.Parameters.AddWithValue("@Width", txtColourWidth.Text)
                            myCmd.Parameters.AddWithValue("@Active", ddlColourActive.SelectedValue)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"FabricColours", lblIdColour.Text, Session("LoginId").ToString(), "Fabric Colour Updated"}
                    settingCfg.Log_Product(dataLog)

                    Response.Redirect("~/setting/product/fabric/detail", False)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            MessageError_AddColour(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showAddColour", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSubmitDeleteColour_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            lblIdColour.Text = txtIdColourDelete.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM FabricColours WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblIdColour.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Response.Redirect("~/setting/product/fabric/detail", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/product/fabric/detail", False)
    End Sub

    Private Sub BindDataFabric(Id As String)
        Try
            Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM Fabrics WHERE Id = '" + Id + "'")

            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/setting/product/fabric", False)
                Exit Sub
            End If

            BindDesign()
            BindTube()

            txtId.Text = myData.Tables(0).Rows(0).Item("Id").ToString()
            txtId.ReadOnly = True

            txtName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
            lblName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()

            ddlType.SelectedValue = myData.Tables(0).Rows(0).Item("Type").ToString()
            lblType.Text = myData.Tables(0).Rows(0).Item("Type").ToString()

            txtGroup.Text = myData.Tables(0).Rows(0).Item("Group").ToString()
            lblGroup.Text = myData.Tables(0).Rows(0).Item("Group").ToString()

            txtComposition.Text = myData.Tables(0).Rows(0).Item("Composition").ToString()
            lblComposition.Text = myData.Tables(0).Rows(0).Item("Composition").ToString()

            ddlPvcLead.SelectedValue = myData.Tables(0).Rows(0).Item("PvcLead").ToString()
            lblPvcLead.Text = myData.Tables(0).Rows(0).Item("PvcLead").ToString()

            txtWeight.Text = myData.Tables(0).Rows(0).Item("Weight").ToString()
            lblWeight.Text = myData.Tables(0).Rows(0).Item("Weight").ToString()

            txtThickness.Text = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Thickness"))
            lblThickness.Text = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Thickness"))

            divFlameReterdantSuccess.Visible = False : divFlameReterdantDanger.Visible = False
            Dim flameReterdant As Integer = Convert.ToInt32(myData.Tables(0).Rows(0).Item("FlameReterdant"))
            ddlFlameReterdant.SelectedValue = flameReterdant
            If flameReterdant = 1 Then
                divFlameReterdantSuccess.Visible = True : divFlameReterdantDanger.Visible = False
            End If
            If flameReterdant = 0 Then
                divFlameReterdantSuccess.Visible = False : divFlameReterdantDanger.Visible = True
            End If

            divGreenguardGoldSuccess.Visible = False : divGreenguardGoldDanger.Visible = False
            Dim greenguardGold As Integer = Convert.ToInt32(myData.Tables(0).Rows(0).Item("GreenguardGold"))
            ddlGreenguardGold.SelectedValue = greenguardGold
            If greenguardGold = 1 Then
                divGreenguardGoldSuccess.Visible = True : divGreenguardGoldDanger.Visible = False
            End If
            If greenguardGold = 0 Then
                divGreenguardGoldSuccess.Visible = False : divGreenguardGoldDanger.Visible = True
            End If

            divNoRailRoadSuccess.Visible = False : divNoRailRoadDanger.Visible = False
            Dim noRailRoad As Integer = Convert.ToInt32(myData.Tables(0).Rows(0).Item("NoRailRoad"))
            ddlNoRailRoad.SelectedValue = noRailRoad
            If noRailRoad = 1 Then
                divNoRailRoadSuccess.Visible = True : divNoRailRoadDanger.Visible = False
            End If
            If noRailRoad = 0 Then
                divNoRailRoadSuccess.Visible = False : divNoRailRoadDanger.Visible = True
            End If

            divActiveSuccess.Visible = False : divActiveDanger.Visible = False
            Dim active As Integer = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Active"))
            ddlActive.SelectedValue = active
            If active = 1 Then
                divActiveSuccess.Visible = True : divActiveDanger.Visible = False
            End If
            If active = 0 Then
                divActiveSuccess.Visible = False : divActiveDanger.Visible = True
            End If

            If Not myData.Tables(0).Rows(0).Item("DesignId").ToString() = "" Then
                Dim designArray() As String = myData.Tables(0).Rows(0).Item("DesignId").ToString().Split(",")
                For Each i In designArray
                    If Not (i.Equals(String.Empty)) Then
                        lbDesign.Items.FindByValue(i).Selected = True
                    End If
                Next
                Dim selectedDesign As DataSet = settingCfg.GetListData("SELECT Designs.Name AS DesignName FROM Fabrics CROSS APPLY STRING_SPLIT(Fabrics.DesignId, ',') AS designArray LEFT JOIN Designs ON designArray.VALUE = Designs.Id WHERE Fabrics.Id = '" + Id + "'")
                Dim selectedText As String = String.Empty
                If Not selectedDesign.Tables(0).Rows.Count = 0 Then
                    For i As Integer = 0 To selectedDesign.Tables(0).Rows.Count - 1
                        Dim designName As String = selectedDesign.Tables(0).Rows(i).Item("DesignName").ToString()
                        selectedText += designName & ", "
                    Next
                End If
                lblProduct.Text = selectedText.Remove(selectedText.Length - 2).ToString()
            End If

            If Not myData.Tables(0).Rows(0).Item("TubeType").ToString() = "" Then
                Dim tubeArray() As String = myData.Tables(0).Rows(0).Item("TubeType").ToString().Split(",")
                For Each i In tubeArray
                    If Not (i.Equals(String.Empty)) Then
                        lbTube.Items.FindByValue(i).Selected = True
                    End If
                Next
                lblTube.Text = myData.Tables(0).Rows(0).Item("TubeType").ToString()
            End If

            ddlFactory.SelectedValue = myData.Tables(0).Rows(0).Item("Factory").ToString()
            lblFactory.Text = myData.Tables(0).Rows(0).Item("Factory").ToString()

            gvListColour.DataSource = settingCfg.GetListData("SELECT *, CASE WHEN Active=1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM FabricColours WHERE FabricId = '" + Id + "' ORDER BY Name ASC")
            gvListColour.DataBind()

            gvListColour.Columns(1).Visible = False
            If Session("LevelName") = "Leader" Or Session("LevelName") = "Member" Then
                gvListColour.Columns(1).Visible = True
            End If

            aEdit.Visible = False
            aDelete.Visible = False
            btnAddColour.Visible = False
            If Session("RoleName") = "Administrator" And Session("LevelName") = "Leader" Then
                aEdit.Visible = True
                aDelete.Visible = True
                btnAddColour.Visible = True
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindDesign()
        lbDesign.Items.Clear()
        Try
            lbDesign.DataSource = settingCfg.GetListData("SELECT UPPER(Id) AS IdText, * FROM Designs WHERE Type <> 'Pricing' ORDER BY Name ASC")
            lbDesign.DataTextField = "Name"
            lbDesign.DataValueField = "IdText"
            lbDesign.DataBind()

            If lbDesign.Items.Count > 1 Then
                lbDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub BindTube()
        lbTube.Items.Clear()
        Try
            lbTube.DataSource = settingCfg.GetListData("SELECT * FROM ProductTubes ORDER BY Name ASC")
            lbTube.DataTextField = "Name"
            lbTube.DataValueField = "Name"
            lbTube.DataBind()

            If lbTube.Items.Count > 1 Then
                lbTube.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub

    Private Sub MessageError_Edit(Show As Boolean, Msg As String)
        divErrorEdit.Visible = Show : msgErrorEdit.InnerText = Msg
    End Sub

    Private Sub MessageError_AddColour(Show As Boolean, Msg As String)
        divErrorAddColour.Visible = Show : msgErrorAddColour.InnerText = Msg
    End Sub

    Private Sub MessageError_Log(Show As Boolean, Msg As String)
        divErrorLog.Visible = Show : msgErrorLog.InnerText = Msg
    End Sub

    Protected Function BindTextLog(Id As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = settingCfg.GetListData("SELECT * FROM Log_Products WHERE Id = '" + Id + "'")
            Dim actionBy As String = thisData.Tables(0).Rows(0).Item("ActionBy").ToString()
            Dim actionDate As String = Convert.ToDateTime(thisData.Tables(0).Rows(0).Item("ActionDate")).ToString("dd MMM yyyy hh:mm")
            Dim description As String = thisData.Tables(0).Rows(0).Item("Description").ToString()

            Dim fullName As String = settingCfg.GetItemData("SELECT FullName FROM CustomerLogins WHERE Id = '" + UCase(actionBy).ToString() + "'")

            result = "<b>" & fullName & "</b> on " & actionDate & ". Action: " & description
        Catch ex As Exception
            result = ""
        End Try
        Return result
    End Function

    Protected Function VisibleAction() As Boolean
        If Session("LevelName") = "Leader" Then
            Return True
        End If
        Return False
    End Function
End Class
