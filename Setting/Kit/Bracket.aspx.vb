Partial Class Setting_Kit_Bracket
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call MessageError(False, String.Empty)

            Call BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnKit_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/kit", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Call BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        Call MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            Call BindData(txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdDelete.Text

            sdsPage.Delete()

            Dim vUserId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(vUserId, Page.Title, "DELETE BRACKET TYPE. ID : " & lblId.Text)

            Call BindData(txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub linkActive_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Dim active As String = row.Cells(4).Text

            Dim newActive As Integer = 0
            If active = "False" Then : newActive = 1 : End If

            lblId.Text = UCase(row.Cells(1).Text)
            lblActive.Text = newActive

            sdsPage.Update()

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "ACTIVE BRACKET TYPE. ID : " & lblId.Text)

            Call BindData(txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            If msgError.InnerText = "" Then
                sdsAddUpdate.Insert()

                Response.Redirect("~/setting/kit/bracket", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnUpdate_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            If msgError.InnerText = "" Then
                lblId.Text = UCase(txtIdEdit.Text).ToString()
                sdsAddUpdate.Update()

                Response.Redirect("~/setting/kit/bracket", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindData(SearchText As String)
        Try
            Dim search As String = String.Empty
            If Not SearchText = "" Then
                search = " WHERE Id LIKE '%" + SearchText + "%' OR Name LIKE '%" + SearchText + "%'"
            End If
            gvList.DataSource = publicCfg.GetListData(String.Format("SELECT * FROM BracketType {0} ORDER BY Name ASC", search))
            gvList.DataBind()
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Function IconActiveOnNames(Active As String, Name As String) As String
        Dim result As String = "<i class='fa-regular text-danger fa-circle-xmark'></i> " + Name
        If Active = "True" Then : Return "<i class='fa-regular text-green fa-circle-check'></i> " + Name : End If
        Return result
    End Function

    Protected Function TextActive(Active As String) As String
         Dim result As String = "<i class='bi bi-arrow-repeat fs-3 me-2 opacity-50'></i>Swicth Activated"
        ' Dim result As String = "Activate"
        ' If Active = "True" Then : Return "Deactivate" : End If
        Return result
    End Function

    Protected Function CssActive(Active As Boolean) As String
        Dim result As String = "dropdown-item"
        ' Dim result As String = "btn btn-sm btn-secondary"
        ' If Active = False Then : result = "btn btn-sm btn-info" : End If
        Return result
    End Function

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub
End Class
