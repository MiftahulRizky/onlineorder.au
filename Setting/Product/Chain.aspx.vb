Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Product_Chain
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/product", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("chainSearch")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        BindDataDesign()
        BindDataTube()
        BindDataControl()
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Chain / Remote"
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
            Exit Sub
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
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
                MessageError_Process(False, String.Empty)
                BindDataDesign()
                BindDataTube()
                BindDataControl()

                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Chain / Remote"

                    Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM Chains WHERE Id = '" + lblId.Text + "'")
                    txtBoeId.Text = myData.Tables(0).Rows(0).Item("BoeId").ToString()
                    txtName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
                    txtDescription.Text = myData.Tables(0).Rows(0).Item("Description").ToString()
                    ddlActive.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Active"))

                    Dim designArray() As String = myData.Tables(0).Rows(0).Item("DesignId").ToString().Split(",")
                    For Each i In designArray
                        If Not (i.Equals(String.Empty)) Then
                            lbDesign.Items.FindByValue(i).Selected = True
                        End If
                    Next

                    Dim tubeArray() As String = myData.Tables(0).Rows(0).Item("TubeType").ToString().Split(",")
                    For Each i In tubeArray
                        If Not (i.Equals(String.Empty)) Then
                            lbTube.Items.FindByValue(i).Selected = True
                        End If
                    Next

                    Dim controlArray() As String = myData.Tables(0).Rows(0).Item("ControlType").ToString().Split(",")
                    For Each i In controlArray
                        If Not (i.Equals(String.Empty)) Then
                            lbControl.Items.FindByValue(i).Selected = True
                        End If
                    Next
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Catch ex As Exception
                    MessageError_Process(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingCfg.GetListData("SELECT * FROM Log_Products WHERE DataId = '" + dataId + "' AND Type='Chains' ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnSubmitDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdDelete.Text
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Chains WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Session("chainSearch") = txtSearch.Text
            Response.Redirect("~/setting/product/chain", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnSubmitProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If Not txtBoeId.Text = "" Then
                If Not IsNumeric(txtBoeId.Text) Then
                    MessageError_Process(True, "BOE ID IS REQUIRED !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                    Exit Sub
                End If

                If txtBoeId.Text < 0 Then
                    MessageError_Process(True, "PLEASE CHECK YOUR BOE ID !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                    Exit Sub
                End If
            End If

            If txtName.Text = "" Then
                MessageError_Process(True, "CHAIN / REMOTE NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                Dim designType As String = String.Empty
                If Not lbDesign.SelectedValue = "" Then
                    Dim selectedDesign As String = String.Empty
                    For Each item As ListItem In lbDesign.Items
                        If item.Selected Then
                            selectedDesign += UCase(item.Value).ToString() & ","
                        End If
                    Next
                    designType = selectedDesign.Remove(selectedDesign.Length - 1).ToString()
                End If

                Dim tubeType As String = String.Empty
                If Not lbTube.SelectedValue = "" Then
                    Dim selectedTube As String = String.Empty
                    For Each item As ListItem In lbTube.Items
                        If item.Selected Then
                            selectedTube += item.Value & ","
                        End If
                    Next
                    tubeType = selectedTube.Remove(selectedTube.Length - 1).ToString()
                End If

                Dim controlType As String = String.Empty
                If Not lbControl.SelectedValue = "" Then
                    Dim selectedControl As String = String.Empty
                    For Each item As ListItem In lbControl.Items
                        If item.Selected Then
                            selectedControl += item.Value & ","
                        End If
                    Next
                    controlType = selectedControl.Remove(selectedControl.Length - 1).ToString()
                End If

                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")
                If lblAction.Text = "Add" Then
                    Dim thisId As String = String.Empty

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Chains OUTPUT INSERTED.Id VALUES (NEWID(), @BoeId, @Name, @DesignId, @TubeType, @ControlType, @Description, @Active)")
                            myCmd.Parameters.AddWithValue("@BoeId", If(String.IsNullOrEmpty(txtBoeId.Text), CType(DBNull.Value, Object), txtBoeId.Text))
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@DesignId", UCase(designType).ToString())
                            myCmd.Parameters.AddWithValue("@TubeType", tubeType)
                            myCmd.Parameters.AddWithValue("@ControlType", controlType)
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            thisId = myCmd.ExecuteScalar().ToString()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"Chains", thisId, Session("LoginId").ToString(), "Chain / Remote Created"}
                    settingCfg.Log_Product(dataLog)

                    Session("chainSearch") = txtSearch.Text
                    Response.Redirect("~/setting/product/chain", False)
                End If
                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE Chains SET BoeId=@BoeId, Name=@Name, DesignId=@DesignId, TubeType=@TubeType, ControlType=@ControlType, Description=@Description, Active=@Active WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@BoeId", If(String.IsNullOrEmpty(txtBoeId.Text), CType(DBNull.Value, Object), txtBoeId.Text))
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@DesignId", UCase(designType).ToString())
                            myCmd.Parameters.AddWithValue("@TubeType", tubeType)
                            myCmd.Parameters.AddWithValue("@ControlType", controlType)
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"Chains", lblId.Text, Session("LoginId").ToString(), "Chain / Remote Updated"}
                    settingCfg.Log_Product(dataLog)

                    Session("chainSearch") = txtSearch.Text
                    Response.Redirect("~/setting/product/chain", False)
                End If

            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Session("chainSearch") = txtSearch.Text
        Response.Redirect("~/setting/product/chain", False)
    End Sub

    Private Sub BindData(SearchText As String)
        Try
            Dim search As String = String.Empty
            If Not SearchText = "" Then
                search = " WHERE Id LIKE '%" + SearchText.Trim() + "%' OR Name LIKE '%" + SearchText.Trim() + "%' OR ControlType LIKE '%" + SearchText.Trim() + "%' OR TubeType LIKE '%" + SearchText.Trim() + "%'"
            End If
            Dim myQuery As String = String.Format("SELECT *, LEFT(TubeType, 20) + ' ....' AS NewTube, LEFT(ControlType, 20) + ' ....' AS NewControl, CASE WHEN Active=1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM Chains {0} ORDER BY Name ASC", search)

            gvList.DataSource = settingCfg.GetListData(myQuery)
            gvList.DataBind()

            btnAdd.Visible = False
            If Session("RoleName") = "Administrator" And Session("LevelName") = "Leader" Then btnAdd.Visible = True
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindDataDesign()
        lbDesign.Items.Clear()
        Try
            lbDesign.DataSource = settingCfg.GetListData("SELECT *, UPPER(Id) AS IdText FROM Designs WHERE Type <> 'Pricing' ORDER BY Name ASC")
            lbDesign.DataTextField = "Name"
            lbDesign.DataValueField = "IdText"
            lbDesign.DataBind()

            If lbDesign.Items.Count > 1 Then
                lbDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub BindDataTube()
        lbTube.Items.Clear()
        Try
            lbTube.DataSource = settingCfg.GetListData("SELECT Name FROM ProductTubes ORDER BY Name ASC")
            lbTube.DataTextField = "Name"
            lbTube.DataValueField = "Name"
            lbTube.DataBind()

            If lbTube.Items.Count > 1 Then
                lbTube.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub BindDataControl()
        lbControl.Items.Clear()
        Try
            lbControl.DataSource = settingCfg.GetListData("SELECT * FROM ProductControls WHERE Active = 1 ORDER BY Name ASC")
            lbControl.DataTextField = "Name"
            lbControl.DataValueField = "Name"
            lbControl.DataBind()

            If lbControl.Items.Count > 1 Then
                lbControl.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub

    Private Sub MessageError_Process(Show As Boolean, Msg As String)
        divErrorProcess.Visible = Show : msgErrorProcess.InnerText = Msg
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
