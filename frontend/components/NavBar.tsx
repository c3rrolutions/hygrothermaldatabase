import { useQuery } from "@apollo/client/react";
import Link from "next/link";
import { useRouter } from "next/router";
import { Button, Menu, Spin } from "antd";
import { LoadingOutlined, UserOutlined } from "@ant-design/icons";
import paths from "../paths";
import {
  CurrentUserDocument,
  CurrentUserPartialFragment,
} from "../queries/currentUser.generated";
import { extractAntiforgeryTokenFromCookie } from "../lib/apollo";
import { Route } from "next";
import { CSSProperties, useMemo } from "react";
import { isTruthy } from "../lib/array";

const firstUserOrLoginItemStyle = { marginLeft: "auto" };

const userLoadingItem = {
  key: "userLoading",
  style: firstUserOrLoginItemStyle,
  label: (
    <Spin indicator={<LoadingOutlined style={{ color: "white" }} spin />} />
  ),
};

const loginOrRegisterItems = (returnTo: string | string[] | undefined) => [
  {
    key: paths.login,
    style: firstUserOrLoginItemStyle,
    label: (
      <Link
        href={{
          pathname: paths.login,
          query: returnTo
            ? { returnTo: returnTo }
            : window.location.pathname != paths.login
              ? { returnTo: window.location.pathname }
              : null,
        }}
      >
        Login
      </Link>
    ),
  },
  {
    key: paths.metabase.register,
    label: (
      <Link
        href={{
          pathname: paths.metabase.register,
          query: returnTo
            ? { returnTo: returnTo }
            : window.location.pathname != paths.metabase.register
              ? { returnTo: window.location.pathname }
              : null,
        }}
      >
        Register
      </Link>
    ),
  },
];

const userItems = (currentUser: CurrentUserPartialFragment) => [
  {
    key: "user",
    label: currentUser.name,
    icon: <UserOutlined />,
    style: firstUserOrLoginItemStyle,
    children: [
      {
        key: paths.userInfo,
        label: <Link href={paths.userInfo}>User Info</Link>,
      },
      currentUser.isAtLeastAssistantManagerOfDatabaseOperator && {
        key: paths.createData,
        label: <Link href={paths.createData}>Create Data</Link>,
      },
      currentUser.isAtLeastAssistantManagerOfDatabaseOperator && {
        key: paths.uploadFile,
        label: <Link href={paths.uploadFile}>Upload File</Link>,
      },
      {
        key: paths.logout,
        label: (
          <form action={paths.logout} method="post">
            <input
              name="__RequestVerificationToken"
              type="hidden"
              value={
                typeof window !== "undefined"
                  ? (extractAntiforgeryTokenFromCookie() ?? "")
                  : ""
              }
            />
            <input
              name="returnTo"
              type="hidden"
              value={
                typeof window !== "undefined" ? window.location.pathname : ""
              }
            />
            <Button type="primary" htmlType="submit">
              Logout
            </Button>
          </form>
        ),
      },
    ].filter(isTruthy),
  },
];

type NavItemProps = {
  path: Route;
  label: string;
};

export type NavBarProps = {
  items: NavItemProps[];
  style?: CSSProperties;
};

export default function NavBar({ items, style }: NavBarProps) {
  const router = useRouter();
  const returnTo = router.query.returnTo;
  const { loading, data } = useQuery(CurrentUserDocument);
  const currentUser = data?.currentUser;

  const mainItems = useMemo(
    () =>
      items.map((item) => ({
        key: item.path,
        label: <Link href={item.path}>{item.label}</Link>,
      })),
    [items],
  );

  const userOrLoginItems = useMemo(
    () =>
      loading
        ? [userLoadingItem]
        : currentUser
          ? userItems(currentUser)
          : loginOrRegisterItems(returnTo),
    [loading, currentUser],
  );

  return (
    <Menu
      mode="horizontal"
      theme="dark"
      selectedKeys={[router.pathname]}
      style={style}
      items={[...mainItems, ...userOrLoginItems]}
    />
  );
}
