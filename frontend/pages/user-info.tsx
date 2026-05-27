import { useQuery } from "@apollo/client/react";
import Layout from "../components/Layout";
import { CurrentUserInfoDocument } from "../queries/currentUser.generated";
import { Skeleton, Result, Descriptions, Typography } from "antd";
import { PageHeader } from "@ant-design/pro-layout";
import paths from "../paths";
import { useQueryHandler } from "../lib/hooks/useQueryHandler";

function Page() {
  const { loading, error, data } = useQuery(CurrentUserInfoDocument);
  useQueryHandler({ error });
  const currentUserInfo = data?.currentUserInfo;

  if (loading) {
    return <Skeleton active avatar title />;
  }

  if (!currentUserInfo) {
    return (
      <Result
        status="500"
        title="500"
        subTitle="Sorry, something went wrong."
      />
    );
  }

  return (
    <Layout>
      <PageHeader
        title={currentUserInfo.name}
        subTitle={currentUserInfo.sub}
        backIcon={false}
      >
        <Descriptions size="small" column={1}>
          <Descriptions.Item label="Metabase">
            <Typography.Link href={paths.metabase.user(currentUserInfo.sub)}>
              {paths.metabase.user(currentUserInfo.sub)}
            </Typography.Link>
          </Descriptions.Item>
          {currentUserInfo.email && (
            <Descriptions.Item label="Email">
              {currentUserInfo.email} (
              {currentUserInfo.emailVerified === true
                ? "Verified"
                : "Unverified"}
              )
            </Descriptions.Item>
          )}
          {currentUserInfo.phoneNumber && (
            <Descriptions.Item label="Phone Number">
              {currentUserInfo.phoneNumber} (
              {currentUserInfo.phoneNumberVerified === true
                ? "Verified"
                : "Unverified"}
              )
            </Descriptions.Item>
          )}
          {currentUserInfo.address && (
            <Descriptions.Item label="Address">
              {currentUserInfo.address.formatted}
            </Descriptions.Item>
          )}
          {currentUserInfo.website && (
            <Descriptions.Item label="Website">
              <Typography.Link href={currentUserInfo.website}>
                {currentUserInfo.website}
              </Typography.Link>
            </Descriptions.Item>
          )}
          {currentUserInfo.roles && (
            <Descriptions.Item label="Roles">
              {currentUserInfo.roles}
            </Descriptions.Item>
          )}
        </Descriptions>
      </PageHeader>
    </Layout>
  );
}

export default Page;
