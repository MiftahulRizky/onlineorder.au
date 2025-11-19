
Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Product_Fabric_Default
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" And Not Session("RoleName") = "Data Entry" Then
            Response.Redirect("~/setting/product", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("fabricSearch")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Add(False, String.Empty)
        BindDesign()
        BindTube()

        Dim thisScript As String = "window.onload = function() { showAdd(); };"
        Try
            ClientScript.RegisterStartupScript(Me.GetType(), "showAdd()", thisScript, True)
            Exit Sub
        Catch ex As Exception
            MessageError_Add(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showAdd()", thisScript, True)
        End Try
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)
                Try
                    Session("fabricDetail") = dataId
                    Session("fabricSearch") = txtSearch.Text
                    Response.Redirect("~/setting/product/fabric/detail", False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingCfg.GetListData("SELECT * FROM Log_Products WHERE DataId = '" + dataId + "' AND Type='Fabrics' ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnSubmitAdd_Click(sender As Object, e As EventArgs)
        MessageError_Add(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showAdd(); };"
        Try
            If txtName.Text = "" Then
                MessageError_Add(True, "NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showAdd()", thisScript, True)
                Exit Sub
            End If

            If ddlType.SelectedValue = "" Then
                MessageError_Add(True, "TYPE REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showAdd()", thisScript, True)
                Exit Sub
            End If

            If txtGroup.Text = "" Then
                MessageError_Add(True, "GROUP REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showAdd()", thisScript, True)
                Exit Sub
            End If

            If txtComposition.Text = "" Then
                MessageError_Add(True, "COMPOSIITON REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showAdd()", thisScript, True)
                Exit Sub
            End If

            If ddlFlameReterdant.SelectedValue = "" Then
                MessageError_Add(True, "FLAME RETERDANT REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showAdd()", thisScript, True)
                Exit Sub
            End If

            If ddlPvcLead.SelectedValue = "" Then
                MessageError_Add(True, "PVC OR LEAD FREE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showAdd()", thisScript, True)
                Exit Sub
            End If

            If ddlGreenguardGold.SelectedValue = "" Then
                MessageError_Add(True, "GREENGUARD GOLD IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showAdd()", thisScript, True)
                Exit Sub
            End If

            If txtWeight.Text = "" Then
                MessageError_Add(True, "WEIGHT IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showAdd()", thisScript, True)
                Exit Sub
            End If

            If txtThickness.Text = "" Then
                MessageError_Add(True, "THICKNESS IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showAdd()", thisScript, True)
                Exit Sub
            End If

            Dim design As String = String.Empty
            For Each item As ListItem In lbDesign.Items
                If item.Selected Then
                    design += UCase(item.Value).ToString() & ","
                End If
            Next
            If design = "" Then
                MessageError_Add(True, "DESIGN TYPE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showAdd()", thisScript, True)
                Exit Sub
            End If

            If ddlFactory.SelectedValue = "" Then
                MessageError_Add(True, "FACTORY IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showAdd()", thisScript, True)
                Exit Sub
            End If

            If msgErrorAdd.InnerText = "" Then
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
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Fabrics VALUES (@Id, @DesignId, @TubeType, @Factory, @Name, @Type, @Group, @Composition, @FlameReterdant, @PvcLead, @GreenguardGold, @Weight, @Thickness, @NoRailRoad, @Active)")
                        myCmd.Parameters.AddWithValue("@Id", thisId)
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

                Dim dataLog As Object() = {"Fabrics", thisId, Session("LoginId").ToString(), "Fabric Created"}
                settingCfg.Log_Product(dataLog)

                Session("fabricSearch") = txtSearch.Text
                Session("fabricDetail") = txtId.Text
                Response.Redirect("~/setting/product/fabric/detail", False)
                Exit Sub
            End If
        Catch ex As Exception
            MessageError_Add(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showAdd()", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSubmitDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Fabrics WHERE Id=@Id DELETE FROM FabricColours WHERE FabricId=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Session("fabricSearch") = txtSearch.Text
            Response.Redirect("~/setting/product/fabric", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Session("fabricSearch") = txtSearch.Text
        Response.Redirect("~/setting/product/fabric", False)
    End Sub

    Private Sub BindData(SearchText As String)
        Try
            Dim search As String = String.Empty

            If Not SearchText = "" Then
                search = " WHERE Id LIKE '%" + SearchText.Trim() + "%' OR Name LIKE '%" + SearchText.Trim() + "%' OR [Group] LIKE '%" + SearchText.Trim() + "%' OR TubeType LIKE '%" + SearchText.Trim() + "%'"
            End If

            Dim thisQuery As String = String.Format("SELECT *, LEFT(TubeType, 20) + ' ....' AS TubeData, CASE WHEN Active=1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM Fabrics {0} ORDER BY CASE WHEN Factory = 'JKT' THEN 1 WHEN Factory = 'Orion' THEN 2 ELSE 0 END, Type ASC", search)

            gvList.DataSource = settingCfg.GetListData(thisQuery)
            gvList.DataBind()

            btnAdd.Visible = False
            If Session("RoleName") = "Administrator" And Session("LevelName") = "Leader" Then btnAdd.Visible = True
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindDesign()
        lbDesign.Items.Clear()
        Try
            lbDesign.DataSource = settingCfg.GetListData("SELECT * FROM Designs WHERE Type <> 'Pricing' ORDER BY Name ASC")
            lbDesign.DataTextField = "Name"
            lbDesign.DataValueField = "Id"
            lbDesign.DataBind()

            If lbDesign.Items.Count > 1 Then
                lbDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
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
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Function BindDetailProduct(Id As String) As String
        Dim result As String = String.Empty
        Try
            Dim hasil As String = String.Empty
            Dim myData As DataSet = settingCfg.GetListData("SELECT Designs.Name AS DesignName FROM Fabrics CROSS APPLY STRING_SPLIT(Fabrics.DesignId, ',') AS designArray LEFT JOIN Designs ON designArray.VALUE = Designs.Id WHERE Fabrics.Id = '" + Id + "'")

            If myData.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To myData.Tables(0).Rows.Count - 1
                    Dim designName As String = myData.Tables(0).Rows(i).Item("DesignName").ToString()
                    hasil &= designName & ", "
                Next
            End If

            If hasil.Length > 2 Then
                hasil = hasil.Remove(hasil.Length - 2)
            End If
            If hasil.Length > 20 Then
                result = hasil.Substring(0, 20) & "..."
            Else
                result = hasil
            End If
        Catch ex As Exception
            result = "ERROR"
        End Try
        Return result
    End Function

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub

    Private Sub MessageError_Add(Show As Boolean, Msg As String)
        divErrorAdd.Visible = Show : msgErrorAdd.InnerText = Msg
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
