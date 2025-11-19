Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Product_Bottom
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
            txtSearch.Text = Session("bottomSearch")
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
            titleProcess.InnerText = "Add Bottom Rail"
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
                    titleProcess.InnerText = "Edit Bottom Rail"

                    Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM Bottoms WHERE Id = '" + lblId.Text + "'")
                    txtBoeId.Text = myData.Tables(0).Rows(0).Item("BoeId").ToString()
                    txtName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
                    txtType.Text = myData.Tables(0).Rows(0).Item("Type").ToString()
                    txtColour.Text = myData.Tables(0).Rows(0).Item("Colour").ToString()
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
                    gvListLogs.DataSource = settingCfg.GetListData("SELECT * FROM Log_Products WHERE DataId = '" + dataId + "' AND Type='Bottoms' ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnSubmitProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If txtName.Text = "" Then
                MessageError_Process(True, "MOUNTING NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtType.Text = "" Then
                MessageError_Process(True, "BOTTOM TYPE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtColour.Text = "" Then
                MessageError_Process(True, "BOTTOM COLOUR IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            Dim selectedDesign As String = String.Empty
            For Each item As ListItem In lbDesign.Items
                If item.Selected Then
                    selectedDesign += UCase(item.Value).ToString() & ","
                End If
            Next
            If selectedDesign = "" Then
                MessageError_Process(True, "DESIGN TYPE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            Dim selectedTube As String = String.Empty
            For Each item As ListItem In lbTube.Items
                If item.Selected Then
                    selectedTube += item.Value & ","
                End If
            Next

            Dim selectedControl As String = String.Empty
            For Each item As ListItem In lbControl.Items
                If item.Selected Then
                    selectedControl += item.Value & ","
                End If
            Next
            If selectedControl = "" Then
                MessageError_Process(True, "CONTROL TYPE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                If txtBoeId.Text = "" Then txtBoeId.Text = 0
                Dim designType As String = selectedDesign.Remove(selectedDesign.Length - 1).ToString()
                Dim tubeType As String = selectedTube.Remove(selectedTube.Length - 1).ToString()
                Dim controlType As String = selectedControl.Remove(selectedControl.Length - 1).ToString()

                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")
                If lblAction.Text = "Add" Then
                    Dim thisId As String = String.Empty

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Bottoms OUTPUT INSERTED.Id VALUES (NEWID(), @BoeId, @Name, @Type, @Colour, @DesignId, @TubeType, @ControlType, @Description, @Active)")
                            myCmd.Parameters.AddWithValue("@BoeId", If(String.IsNullOrEmpty(txtBoeId.Text), CType(DBNull.Value, Object), txtBoeId.Text))
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Type", txtType.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Colour", txtColour.Text.Trim())
                            myCmd.Parameters.AddWithValue("@DesignId", designType)
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

                    Dim dataLog As Object() = {"Bottoms", thisId, Session("LoginId").ToString(), "Bottom Rail Created"}
                    settingCfg.Log_Product(dataLog)

                    Session("bottomSearch") = txtSearch.Text
                    Response.Redirect("~/setting/product/bottom", False)
                End If
                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE Bottoms SET BoeId=@BoeId, Name=@Name, Type=@Type, Colour=@Colour, DesignId=@DesignId, TubeType=@TubeType, ControlType=@ControlType, Description=@Description, Active=@Active WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@BoeId", If(String.IsNullOrEmpty(txtBoeId.Text), CType(DBNull.Value, Object), txtBoeId.Text))
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Type", txtType.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Colour", txtColour.Text.Trim())
                            myCmd.Parameters.AddWithValue("@DesignId", designType)
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

                    Dim dataLog As Object() = {"Bottoms", lblId.Text, Session("LoginId").ToString(), "Bottom Rail Updated"}
                    settingCfg.Log_Product(dataLog)

                    Session("bottomSearch") = txtSearch.Text
                    Response.Redirect("~/setting/product/bottom", False)
                End If
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSubmitDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdDelete.Text
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Bottoms WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Session("bottomSearch") = String.Empty
            Response.Redirect("~/setting/product/bottom", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Session("bottomSearch") = txtSearch.Text
        Response.Redirect("~/setting/product/bottom", False)
    End Sub

    Private Sub BindData(SearchText As String)
        Try
            Dim search As String = String.Empty

            If Not SearchText = "" Then
                search = " WHERE BoeId LIKE '%" + SearchText + "%' OR Name LIKE '%" + SearchText + "%'"
            End If
            Dim thisString As String = String.Format("SELECT *, CASE WHEN Active = 1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM Bottoms {0} ORDER BY Name ASC", search)

            gvList.DataSource = settingCfg.GetListData(thisString)
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
            lbDesign.DataSource = settingCfg.GetListData("SELECT UPPER(Id) AS IdValue, * FROM Designs WHERE Type <> 'Pricing' ORDER BY Name ASC")
            lbDesign.DataTextField = "Name"
            lbDesign.DataValueField = "IdValue"
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
            lbTube.DataSource = settingCfg.GetListData("SELECT * FROM ProductTubes WHERE Active = 1 ORDER BY Name ASC")
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

    Protected Function BindDetailProduct(Id As String) As String
        Dim result As String = String.Empty
        Try
            Dim hasil As String = String.Empty

            Dim myData As DataSet = settingCfg.GetListData("SELECT Designs.Name AS DesignName FROM Bottoms CROSS APPLY STRING_SPLIT(Bottoms.DesignId, ',') AS designArray LEFT JOIN Designs ON designArray.VALUE = Designs.Id WHERE Bottoms.Id = '" + Id + "'")
            If Not myData.Tables(0).Rows.Count = 0 Then
                For i As Integer = 0 To myData.Tables(0).Rows.Count - 1
                    Dim designName As String = myData.Tables(0).Rows(i).Item("DesignName").ToString()
                    hasil += designName & ", "
                Next
            End If
            result = hasil.Remove(hasil.Length - 2).ToString()
        Catch ex As Exception
            result = "ERROR"
        End Try
        Return result
    End Function

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
