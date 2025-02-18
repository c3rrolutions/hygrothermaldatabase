import { Form, Input, Button, Row, Col, Card, Typography, Upload } from "antd";
import { UploadOutlined } from "@ant-design/icons";
import Layout from "../components/Layout";
import { RcFile } from "antd/lib/upload/interface";

const layout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 16 },
};

// How to customize `Upload` was inspired by the last post in https://www.it-swarm.com.de/de/javascript/wie-sollte-customrequest-der-ant-design-upload-komponente-festgelegt-werden-um-mit-einem-xmlhttprequest-zusammenzuarbeiten/835857426/
// Other information can be found on
// https://ant.design/components/form/#components-form-demo-validate-other
// https://ant.design/components/upload/#onChange
// https://levelup.gitconnected.com/managing-file-uploads-with-ant-design-6d78e592f2c4
// https://codesandbox.io/s/vy7677x3wl?file=/index.js
// https://stackoverflow.com/questions/54845951/react-antdesign-add-uploaded-images-to-formdata
function Page() {
  const [form] = Form.useForm();

  const constructFileUploadAction = (_file: RcFile) =>
    `/api/upload-file?getHttpsResourceUuid=${encodeURIComponent(
      form.getFieldValue("getHttpsResourceUuid")
    )}`;

  return (
    <Layout>
      <Row justify="center">
        <Col>
          <Card title="Upload File">
            <Typography.Paragraph style={{ maxWidth: 768 }}>
              For an existing GET HTTPS resource entry in the database, upload a
              file with the content for that resource.
            </Typography.Paragraph>
            <Form {...layout} form={form} name="basic">
              <Form.Item
                label="GET HTTPS Resource UUID"
                name="getHttpsResourceUuid"
                rules={[
                  {
                    required: true,
                  },
                ]}
              >
                <Input />
              </Form.Item>

              <Form.Item name="file" label="File">
                <Upload
                  action={constructFileUploadAction}
                  withCredentials
                  listType="text"
                >
                  <Button icon={<UploadOutlined />}>
                    Select File to Upload It
                  </Button>
                </Upload>
              </Form.Item>
            </Form>
          </Card>
        </Col>
      </Row>
    </Layout>
  );
}

export default Page;
